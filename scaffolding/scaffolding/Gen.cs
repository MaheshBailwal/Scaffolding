using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scaffolding
{
    public class Gen
    {
        const string rootFolder = "C:\\Dev\\azure-git\\HeavyweightScheduler";
        string entityName = "customer product";
      public static  void Start(string rootFolder, string entityName)
        {

            var _entityNamePascal = "";
           var arr = entityName.Split(' ');
            var _fileName = "";
            var _entityNameSmallCase = "";
            var _routePrefix = "";
            const string tmp = "template_";
            const string fileNameToReplace = "#fileName#";
            const string entityNameToReplace = "#entityName#";
            const string entityNameSmToReplace = "#entitynamesmallcase#";

            foreach (var word in arr)
            {
                var word1 = word.ToLower();
                _entityNameSmallCase += word1;
                word1 = word1.FirstCharToUpper();
                _entityNamePascal += word1;
                _fileName += word.ToLower() + "_";
                _routePrefix+= word.ToLower() + "-";
            }

            _fileName = _fileName.Trim('_');
            _routePrefix = _routePrefix.Trim('-');


            var files = Directory.GetFiles(rootFolder, "", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);


                if(fileName.Contains(tmp) && fileName.EndsWith(".txt"))
                {
                    var fileContent = File.ReadAllText(file);
                 fileContent =   fileContent.Replace(fileNameToReplace, _fileName).
                        Replace(entityNameToReplace, _entityNamePascal).
                        Replace(entityNameSmToReplace, _entityNameSmallCase);
                  var newFileNmae =  file.Replace(tmp, _fileName + "_").Replace(".txt",".py");  
                    
                    File.WriteAllText(newFileNmae, fileContent);
                }

                if (fileName.EndsWith("lambda_function.py")){
                    var fileContent = File.ReadAllText(file);
                    fileContent = fileContent.Replace("#template_routs", $",{_fileName}_routs#template_routs")
                        .Replace("#template_include", $"app.include_router({_fileName}_routs.router,prefix=\"/{_routePrefix}\") {Environment.NewLine}#template_include");

                    File.WriteAllText(file, fileContent);
                }


                if (fileName.EndsWith("response_encoder.py"))
                {
                    var fileContent = File.ReadAllText(file);
                    fileContent = fileContent.Replace("#template-con", $"if isinstance(obj, {_entityNamePascal}DetailDto): return obj.__dict__ {Environment.NewLine} \t\t#template-con")
                        .Replace("#template-import", $"from domain.dtos.{_fileName}_dtos import * {Environment.NewLine}#template-import");
               
                    File.WriteAllText(file, fileContent);
                }
            }



        }
    }

    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input[0].ToString().ToUpper() + input.Substring(1)
            };
    }
}
