using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

    public abstract class StringOperation : INotifyPropertyChanged
    {
        public StringArgs Args { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public abstract string Preset();

        public abstract string Operate(string origin);

        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract StringOperation Clone();
        public abstract void Config();



    }

    public class ReplaceOperation : StringOperation
    {
        public override string Preset()
        {
            var args = Args as ReplaceArgs;
            return $"Replace {args.From} {args.To}";
        }
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
        public override string Preset()
        {
            var args = Args as NewCaseArgs;
            return $"NewCase {args.TypeNewCase}";
        }
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
    public class UniqueNameOperation : StringOperation, INotifyPropertyChanged
    {
        public override string Preset()
        {
            
            return $"UniqueName";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public override string Operate(string origin)
        {
          
            try
            {
                string tail="";
                for (int i = origin.Length - 1; i >= 0; i--)
                {
                    if (origin[i] == '.')
                    {
                        tail = origin.Substring(i , origin.Length-i );
                        break;
                    }
                }
                System.Guid newGuid = Guid.NewGuid();
               
                return newGuid.ToString("D")+ tail;
            }
            catch (Exception ex)
            {

                return origin;
            }

        }
        public override string Name
        {
            get
            {

                return "Unique Name";
            }
        }
        public override string Description
        {
            get
            {

                return $"Replate Name File to Guild";
            }

        }
        public override StringOperation Clone()
        {

            return new UniqueNameOperation();
           
        }
        public override void Config()
        {

        }
    }
    public class MoveOperation  : StringOperation
    {
        public override string Preset()
        {

            return "Move";
        }
       
        public override string Operate(string origin)
        {
            string tail = "";
            for (int i = origin.Length - 1; i >= 0; i--)
            {
                if (origin[i] == '.')
                {
                    tail = origin.Substring(i, origin.Length - i);
                    break;
                }
            }
            string ISBN="";
            int postISBN=0;
            string regexISBN = @"(\d|-){13}";
            Match match = Regex.Match(origin, regexISBN);         
            ISBN = match.Value;
            postISBN = match.Index;

            if (postISBN == 0) return (origin.Substring(13, origin.Length - 13 - tail.Length) + ISBN + tail);
            else return (ISBN + origin.Substring(0, origin.Length - 13 - tail.Length)+tail) ;

            
          
        }
        public override string Name
        {
            get
            {

                return "Move";
            }
        }
        public override string Description
        {
            get
            {

                return $"Move ISBN - NameFile";
            }

        }
        public override StringOperation Clone()
        {

            return new MoveOperation();

        }
        public override void Config()
        {

        }
    }
}
