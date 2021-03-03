using System;
using System.Collections.Generic;
using System.Drawing;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessagesManager.Tests
{
    [TestClass]
    public class TwitterScreenshotterTests
    {
        private readonly Func<TwitterScreenshotter> _screenshotterFactory;

        public TwitterScreenshotterTests()
        {
            _screenshotterFactory = () => new TwitterScreenshotter(new WebDriverFactory(new TwitterScreenshotterConfig{ UseLocalChromeDriver = true, RemoteChromeUrl = "http://localhost:4444/wd/hub/"}).Create());
        }
        
        [TestMethod]
        public void TestTextTweet()
        {
            Test(new Update
            {
                Url = "https://twitter.com/IsraelPolls/status/1362480543733014537",
                Content = "סקר פאנלס עבור 'מעריב':\nהליכוד 28, יש עתיד 18, תקווה חדשה 15, ימינה 12, הרשימה המשותפת 9, ש\"ס 8, ישראל ביתנו 8, יהדות התורה 7, הציונות הדתית 5, העבודה 5, מרצ 5. \nמתחת לאחוז החסימה:\nכחול לבן 2.5%, רע\"מ 2.1%, הכלכלית 1.9%. \nגושים: \nנתניהו 48, בנט 12, לא נתניהו 60.",
                IsReply = false,
                Media = new List<IMedia>()
            });
        }
        
        [TestMethod]
        public void TestPhotoTweet()
        {
            Test(new Update
            {
                Url = "https://twitter.com/Ayelet__Shaked/status/1363400109929684993",
                Content = "מנקים את החופים. \n@AmichaiChikli\n מספר 5 בימינה בחוף אפולוניה.\nלכו לחופים, הם צריכים אותנו, בחמש דקות עבודה ממלאים שק של זפת.",
                IsReply = false,
                Media = new List<IMedia> { new Photo("") }
            });
        }
        
        [TestMethod]
        public void TestAlbumTweet()
        {
            Test(new Update
            {
                Url = "https://twitter.com/yairlapid/status/1362479265762189313",
                Content = "ערב טוב לתושבי באר שבע, עומר, מיתר🟠 התחלנו.",
                IsReply = false,
                Media = new List<IMedia> { new Photo("") }
            });
        }
        
        [TestMethod]
        public void TestReplyTweet()
        {
            Test(new Update
            {
                Url = "https://twitter.com/kann_news/status/1363407092963508230",
                Content = "יו\"ר ועדת חוקה אשר: \"נוטה לא לאשר את המשך חיוב הבידוד במלוניות לחוזרים מחו\"ל, ואם כן, אז רק לזמן קצר מאוד. אין לכך הצדקה. רק שליש נשארים בפועל במלוניות - חלקם מקבלים פטור, אחרים מחוסנים. אפשר להשתמש במכשיר אלקטרוני. על המשטרה לאכוף את הבידוד בבית\"\n@ZeevKam",
                IsReply = true,
                Media = new List<IMedia>()
            }).Save("../../../test.png");
        } 
        
        [TestMethod]
        public void TestReplyVideoTweet()
        {
            Test(new Update
            {
                Url = "https://twitter.com/kann_news/status/1366432842503360519",
                Content = "פיילוט הצמידים הדיגיטלייים | תיעוד: אזור חלוקת הצמידים בנתב\"ג\n@sharonidan\n \nhttp://bit.ly/3q9Rhvc",
                IsReply = true,
                Media = new List<IMedia> { new Photo("") }
            }).Save("../../../test.png");
        }

        [TestMethod]
        public void TestReplyUrlTweet()
        {
            Test(new Update
            {
                Url = "https://twitter.com/kann_news/status/1366648731408490500",
                Content = "תיעוד בלעדי: 48 שעות במיון הפסיכיאטרי | פברואר בכאן חדשות\n@alon_sharvit_",
                IsReply = true,
                Media = new List<IMedia> { new Photo("") }
            }).Save("../../../test.png");
        }

        [TestMethod]
        public void TestQuoteTweet()
        {
            Test(new Update
            {
                Url = "https://twitter.com/bezalelsm/status/1363360010298875907",
                Content = "נפתלי, התחייבות לממשלת ימין זה לא להיות ילד כאפות. חוץ מהשיח הלא ראוי, מה שבעיקר מפחיד זו ההבנה שממשלת ימין מעוררת אצלך בוז. \n\nחבל. גם על השפה וגם על הבלבול בדרך.",
                IsReply = false,
                Media = new List<IMedia>()
            });
        }
        
        private Bitmap Test(Update update)
        {
            Bitmap screenshot = _screenshotterFactory().Screenshot(update);

            Assert.IsNotNull(screenshot);

            return screenshot;
        }
    }
}