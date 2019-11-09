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
    public class NewCaseArgs : StringArgs, INotifyPropertyChanged
    {
        public string TypeNewCase { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;


        // 0 AllUpCase
        // 1 AllLowCase
        // 2 FistWordUpCase
    }
    public class ReplaceArgs : StringArgs
    {

        public string From { get; set; }
        public string To { get; set; }

       
    }

    //public class NewCaseArgs : StringArgs
    //{
    //    public string From { get; set; }
    //}

    public abstract class StringOperation:INotifyPropertyChanged
    {
        public StringArgs Args { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract string Operate(string origin);

        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract StringOperation Clone();
        public abstract void Config();
       


}

    public class ReplaceOperation : StringOperation
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

    public class NewCaseOperation : StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Operate(string origin)
        {
            var filename = System.IO.Path.GetFileName(origin);
            
            var args = Args as NewCaseArgs;
            string result;
            if (args.TypeNewCase == "AllUpCase")
            {
                result = filename.ToUpper();
                return result;
            }
            else if (args.TypeNewCase == "AllLowCase")
            {
                result = filename.ToLower();
                return result;
            }
            else
            {
                if (String.IsNullOrEmpty(filename))
                    return filename;

                result = "";

                //lấy danh sách các từ  

                string[] words = filename.Split(' ');

                foreach (string word in words)
                {
                    // từ nào là các khoảng trắng thừa thì bỏ  
                    if (word.Trim() != "")
                    {
                        if (word.Length > 1)
                            result += word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower() + " ";
                        else
                            result += word.ToUpper() + " ";
                    }

                }
                return result.Trim();
              
               
            }
        }
        
        public override string Name
        {
            get
            {
              
                return "New Case";
            }
        }
        public override string Description
        {
            get
            {
                var args = Args as NewCaseArgs;
                return $"Type: {args.TypeNewCase}";
            }

        }
        public override StringOperation Clone()
        {
            var args = Args as NewCaseArgs;
            return new NewCaseOperation()
            {
                Args = new NewCaseArgs()
                {
                    TypeNewCase = args.TypeNewCase
                }
            };
        }
        public override void Config()
        {
            var args = Args as NewCaseArgs;
            var screen = new NewCaseConfigDialog(args);
            if (screen.ShowDialog() == true)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            }
        }

    }
}
