//using System;
//using System.IO;
//using System.Reflection;
//using Newtonsoft.Json.Converters;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Collections;

//namespace ApexUtilLibTest
//{
//    public class TestDataService
//    {

//        public TestDataService()
//        {

//        }

//        /// <summary>
//        /// Loads the test file.
//        /// </summary>
//        /// <returns>The test file.</returns>
//        /// <param name="path">Path.</param>
//        public IEnumerable<TestParam> LoadTestFile(string path)
//        {
//            try
//            {
//                ///Use below code for relative path
//                ///var projectPath = AppDomain.CurrentDomain.BaseDirectory.Replace("bin/Debug/", "");
//                ///path = path.Replace(".json", "");
//                ///path = Path.Combine(Path.GetDirectoryName(projectPath),  jsonFileName + ".json");

//                using (StreamReader reader = new StreamReader(path))
//                {
//                    string _result = reader.ReadToEnd();

//                    var result = JsonConvert.DeserializeObject<IEnumerable<TestParam>>(_result);

//                    return result;
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//    }
//}
