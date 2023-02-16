using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JFramework
{
    internal class CoroutineWrapper<T>
    {
        private readonly CoroutineAwaiter<T> awaiter;
        private readonly Stack<IEnumerator> processStack;

        public CoroutineWrapper(IEnumerator coroutine, CoroutineAwaiter<T> awaiter)
        {
            processStack = new Stack<IEnumerator>();
            processStack.Push(coroutine);
            this.awaiter = awaiter;
        }

        public IEnumerator Run()
        {
            while (true)
            {
                IEnumerator topWorker = processStack.Peek();
                bool isDone;
                try
                {
                    isDone = !topWorker.MoveNext();
                }
                catch (Exception e)
                {
                    List<Type> objectTrace = TraceObject(processStack);
                    awaiter.Complete(default, objectTrace.Any() ? new Exception(TraceMessage(objectTrace), e) : e);
                    yield break;
                }

                if (isDone)
                {
                    processStack.Pop();

                    if (processStack.Count == 0)
                    {
                        awaiter.Complete((T)topWorker.Current, null);
                        yield break;
                    }
                }

                if (topWorker.Current is IEnumerator current)
                {
                    processStack.Push(current);
                }
                else
                {
                    yield return topWorker.Current;
                }
            }
        }

        private string TraceMessage(List<Type> objTrace)
        {
            StringBuilder result = new StringBuilder();

            foreach (Type objType in objTrace)
            {
                if (result.Length != 0)
                {
                    result.Append(" -> ");
                }

                result.Append(objType);
            }

            result.AppendLine();
            return "异步错误追踪" + result;
        }

        private static List<Type> TraceObject(IEnumerable<IEnumerator> enumerators)
        {
            List<Type> objTrace = new List<Type>();

            foreach (var enumerator in enumerators)
            {
                BindingFlags attr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
                FieldInfo field = enumerator.GetType().GetField("$this", attr);
                if (field == null)
                {
                    continue;
                }

                Object obj = field.GetValue(enumerator);
                if (obj == null)
                {
                    continue;
                }

                Type objType = obj.GetType();
                if (!objTrace.Any() || objType != objTrace.Last())
                {
                    objTrace.Add(objType);
                }
            }

            objTrace.Reverse();
            return objTrace;
        }
    }
}