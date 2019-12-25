using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCBasic
{
    public interface IObjectValue : IDisposable
    {
        string PreferredName { get; }
        BCValue GetValue(string name);
        void SetValue(string name1, BCValue value);
        IList<string> GetNames();
        void InitializeForRun(); // Called before each run 
        void RunComplete(); // The program has finished running and services can be cleaned up

        // IObjectValue can also call a function by name
        Task<RunResult.RunStatus> RunAsync(BCRunContext context, string name, IList<IExpression> ArgList, BCValue Retval);
    }

    public interface IExpression
    {
        Task<BCValue> EvalAsync(BCRunContext context);
    }

    public class RunResult
    {
        public RunResult()
        {
        }
        public RunResult(string str)
        {
            Result = new BCValue(str);
        }
        public enum RunStatus { OK, ErrorContinue, ErrorStop }; // ErrorStop means we can't keep trying.  ErrorContinue means we can.
        // ErrorContinue is used for when an object doesn't have the requested method
        // It's also used when a method is being called 'correctly' but the method can't run
        // For example, when calling g.UseScale ("meter") and there's no such scale.

        public RunStatus Status = RunStatus.OK;
        public BCValue Result = new BCValue("");
        public string Error = "";
        public string Message = "";
        public string GetMessage(string defaultValue)
        {
            if (Message == null || Message == "") return defaultValue;
            return Message;
        }
        public override string ToString() { if (Status == RunStatus.OK) return Result.AsString; return Error; }
    }

    public class BCRunContext
    {

    }

    public class BCObjectUtilities
    {
        public static bool CheckArgs(int min, int max, string name, IList<IExpression> ArgList, BCValue Retval)
        {
            if (ArgList.Count < min)
            {
                if (min == max)
                    Retval.SetError(1, $"{name} requires {min} args, not {ArgList.Count}.");
                else
                    Retval.SetError(1, $"{name} requires {min} to {max} args, not {ArgList.Count}.");
                return false;
            }
            if (ArgList.Count > max)
            {
                if (min == max)
                    Retval.SetError(1, $"{name} requires {min} args, not {ArgList.Count}.");
                else
                    Retval.SetError(1, $"{name} requires {min} to {max} args, not {ArgList.Count}.");
                return false;
            }
            return true;
        }

        public static async Task<bool> CheckArgValue(int index, string argName, int min, int max, BCRunContext context, string name, IList<IExpression> ArgList, BCValue Retval)
        {
            if (index >= ArgList.Count)
            {
                Retval.SetError(1, $"{name} is missing the {argName} argument.");
                return false;
            }
            double value = (await ArgList[index].EvalAsync(context)).AsDouble;
            return CheckValueRange(name, value, argName, min, max, Retval);
        }
        // name and argName are used only for error messages
        public static bool CheckValueRange(string name, double value, string argName, double min, double max, BCValue Retval)
        {
            if (!Double.IsNaN(min) && value < min)
            {
                Retval.SetError(1, $"{name} {argName} argument is {value} but must be >= {min}");
                return false;
            }
            if (!Double.IsNaN(max) && value > max)
            {
                Retval.SetError(1, $"{name} {argName} argument is {value} but must be <= {max}");
                return false;
            }
            return true;
        }
    }

}
