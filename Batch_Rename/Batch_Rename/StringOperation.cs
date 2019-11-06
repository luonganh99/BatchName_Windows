using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Batch_Rename
{
    public class StringArgs 
    {
    }
    
    public class ReplaceArgs: StringArgs
    {

        public string From { get; set; }
        public string To { get; set; }

    }

    //public class NewCaseArgs : StringArgs
    //{
    //    public string From { get; set; }
    //}

    public abstract class StringOperation
    {
        public StringArgs Args {get ; set;}
        public abstract string Operate(string origin);

        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract StringOperation Clone();
        public abstract void Config();


    }

    public class ReplaceOperation: StringOperation
    {
        public override string Operate(string origin)
        {
            var args = Args as ReplaceArgs;
            var from = args.From;
            var to = args.To;

 	        return origin.Replace(from, to);
        }

        public override string Name
        {
            get
            {
                return "Replace";
            }
        }
        public override string Description
        {
            get 
            {
                var args = Args as ReplaceArgs; 
                return String.Format("Replace from {0} to {1}", args.From, args.To);
            }
        }

        public override StringOperation Clone()
        {
            var oldArgs = Args as ReplaceArgs;
            return new ReplaceOperation()
            {
                Args = new ReplaceArgs()
                {
                    From = oldArgs.From,
                    To = oldArgs.To
                }
            };
        }

        public override void Config()
        {
            var screen = new ReplaceConfigDialog(Args);
            if (screen.ShowDialog() == true)
            {
            }
        }
        //public event PropertyChangedEventHandler PropertyChanged;

        //private void Notify(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

    }

    //public class NewCaseOperation : StringOperation
    //{
    //    public override string Operate(string origin)
    //    {
    //        var args = Args as NewCaseArgs;
    //        var from = args.From;

            


    //    }
    //}
}
