using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoreCard.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace StoreCard.Controllers
{
    public class HomeController : Controller
    {
        DataManager db = new DataManager();
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult About()
        {
            var cardSearch = from card in db.GetCards() select card;
            ViewBag.CountCard = cardSearch.Count();
            var topCard = cardSearch.OrderByDescending(c => c.Amount).Take(5).ToList();
            return View(topCard);
        }
        public ActionResult SaveToPDF(Card cart)
        {
            var cardSearch = from card in db.GetCards() select card;
            ViewBag.CountCard = cardSearch.Count();
            var topCard = cardSearch.OrderByDescending(c => c.Amount).Take(5).ToList();
            String htmlText = "<br><br><h1><font  " +
                             " color=\"#0000FF\"><b><i>Top 5 cards   " +
                             " </i></b></font> </h1><br>" +
                             "<table border='1' bgcolor='#999777'>" +
                             "<tr><th style='color:white; text-align:center' width='20%'>Card number</th>" +
                             "<th style='color:white;text-align:center;width:80%'>Amount</th></table>";
            String ja = " ";
            bool prov = true;
            foreach (Card card in topCard)
            {
                if (prov)
                {
                    String lines = "<table><tr><td style='text-align:center' border='1' bgcolor='#d1d1d1' width='20%'>" +
                                            card.CardId +
                                        "</td><td style='text-align:center'border='1' bgcolor='#d1d1d1' width='80%'>" +
                                            card.Amount +
                                        "</td></tr></table>";
                    ja += lines;
                    prov = false;
                }
                else
                {
                    String lines = "<table><tr><td style='text-align:center' border='1' bgcolor='#afafaf' width='20%'>" +
                                            card.CardId +
                                        "</td><td style='text-align:center'border='1' bgcolor='#afafaf' width='80%'>" +
                                            card.Amount +
                                        "</td></tr></table>";
                    ja += lines;
                    prov = true;
                }
            }

            htmlText += ja;
            HTMLToPdf(htmlText, "PDFfile.pdf");
            return RedirectToAction("Index");
        }
        public void HTMLToPdf(string HTML, string FilePath)
        {
            Document document = new Document();

            PdfWriter.GetInstance(document, new FileStream(Request.PhysicalApplicationPath + "\\Chap0101.pdf", FileMode.Create));
            document.Open();
            Image pdfImage = Image.GetInstance(Server.MapPath("\\store.png"));

            pdfImage.ScaleToFit(100, 50);

            pdfImage.Alignment = iTextSharp.text.Image.UNDERLYING; pdfImage.SetAbsolutePosition(180, 760);

            document.Add(pdfImage);
            iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();
            iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
            hw.Parse(new StringReader(HTML));
            document.Close();
            Response.ContentType = "application/pdf";

            //Set default file Name as current datetime
            Response.AddHeader("content-disposition", "attachment; filename=" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
            System.Web.HttpContext.Current.Response.Write(document);
            ShowPdf("\\Chap0101.pdf");
        }
        private void ShowPdf(string s)
        {
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "inline;filename=" + s);
            Response.ContentType = "application/pdf";
            Response.WriteFile(s);
            Response.Flush();
            Response.Clear();
        }
        public ActionResult SearchId(string cardId)
        {
            var cardSearch = from card in db.GetCards() select card;
            if (!String.IsNullOrEmpty(cardId))
            {
                int it = Convert.ToInt16(cardId);
                cardSearch = cardSearch.Where(c => c.CardId == it);

            }
            
            if (cardSearch.Count() == 0 || String.IsNullOrEmpty(cardId))
                return RedirectToAction("Create");
            return View(cardSearch);
        }
        [HttpGet]
        public ActionResult AddToCard(int id)
        {
            return View(db.GetCard(id));
        }

        [HttpPost]
        public ActionResult AddToCard(Card card, string add)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(add))
                    return RedirectToAction("AddToCard");
                Card old = db.GetCard(card.CardId);
                double money = Convert.ToDouble(add);
                double mmoneyToCard = (double)old.Amount;
                if (mmoneyToCard >= 500 && mmoneyToCard <= 2000)
                    money *= 0.98;
                if (mmoneyToCard >= 2000.01 && mmoneyToCard <= 5000)
                    money *= 0.95;
                if (mmoneyToCard >= 5000.01)
                    money *= 0.93;
                card.Amount += (decimal)money;
                old.Amount += card.Amount;
                db.SaveCard(old);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Card card, string add)
        {
            if (String.IsNullOrEmpty(add))
                return RedirectToAction("Create");
                if (ModelState.IsValid)
                {
                    double money = Convert.ToDouble(add);
                    if (money >= 200)
                    {
                        card.Amount = (decimal)money;
                        db.Save(card);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            
            return View(card);
        }
    }
}
