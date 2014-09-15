using System;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;

namespace ACGArtImageBatchDone
{
    
    class Program
    {
        public static string SaveDiskPath = "D:\\ACGART\\";
        public static string ACGHost = "http://acg.sugling.in/json_daily.php?device=iphone5&pro=yes&user=yes&sexyfilter=no&version=m.4.4.11";
        public static ArrayList imgs = new ArrayList();
        public static int imgsNum = 0;
        public static int getNum = 0;
        static void Main(string[] args)
        {
            checkDiskPath();
            fetchImageList();
            ThreadDown(15);
        }
        //check path
        static void checkDiskPath()
        {
            if (!Directory.Exists(SaveDiskPath))
            {
                Directory.CreateDirectory(SaveDiskPath);
            }
        }

        static void fetchImageList()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ACGHost) ;
            request.UserAgent = "ACGArt/4.4.11 CFNetwork/672.1.15 Darwin/14.0.0";
            request.Accept = "*/*";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("Link OK \nRead Json Start");
            
            //get and read json
            Stream jsonget = response.GetResponseStream();
            string content,content1;
            using (StreamReader reader = new StreamReader(jsonget, Encoding.Default))
            {
                content1= content = reader.ReadToEnd();

            }
            var matches = Regex.Matches(content, "[\\[,\\,]\\\"[0-9]*\\.jpg");
            var matches1 = Regex.Matches(content1, "\\d+");
            imgsNum = Convert.ToInt32(matches1[0].ToString());
            foreach (Match match in matches)
            {
                var result = match.Value.ToString();
                result = result.Substring(3, result.Length - 3);
                imgs.Add(result);
            }
            if(imgs.Count==imgsNum)
            {
                Console.WriteLine("Donwload allImgs is:"+imgsNum);

            }

            

        }

        static void downjpg(object filename)
        {
            string tempFileName = SaveDiskPath;
            tempFileName = tempFileName + filename;
            if(System.IO.File.Exists(tempFileName))
            {
                Console.WriteLine(filename+" Exist");
                return;
            }
            else
            {
                Console.WriteLine("DonwLoad " + filename);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://acg.sugling.in/_uploadfiles/iphone5/640/1" + filename);
                request.UserAgent = "ACGArt/4.4.11 CFNetwork/672.1.15 Darwin/14.0.0";
                request.Accept = "*/*";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream image = response.GetResponseStream();
                int length = (int)response.ContentLength;
                BinaryReader br = new BinaryReader(image);
                FileStream fs = File.Create(tempFileName);
                fs.Write(br.ReadBytes(length), 0, length);
                br.Close();
                fs.Close();
            }
        }

        static void ThreadDown(int num)
        {
            int temp = num;
            
            while (getNum != imgsNum)
            {
                for (int i = 1; i <= num; i++)
                {
                    ParameterizedThreadStart ParStart = new ParameterizedThreadStart(downjpg);
                    Thread run = new Thread(ParStart);
                    object o = imgs[getNum];
                    getNum++;
                    run.IsBackground=true;
                    run.Start(o);
                }
            }
            int fileNum = Directory.GetFiles(SaveDiskPath, "*.jpg").Length;
            Console.WriteLine("DownLoad Over,File:"+fileNum);
            Console.ReadKey();
            
        }
    }

}
