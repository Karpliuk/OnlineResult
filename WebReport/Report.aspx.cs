using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebReport.ServiceReference;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Collections;
using NLog;
using System.Configuration;
using System.Globalization;
using System.Reflection.Emit;
using System.Web.DynamicData;

namespace WebReport
{
    public partial class Report : System.Web.UI.Page
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        protected override void InitializeCulture()
        {
            try
            {
                string lang = Page.RouteData.Values["lang"] as string ?? "auto";
                // string lang = Request.QueryString["lang"] as string ?? "auto"; // Get 
                UICulture = lang;
                Culture = lang;
                base.InitializeCulture();
            }
            catch (Exception ex)
            {
               // logger.Warn(ex.Message);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            WarningMessageCulture();          
            if (!IsPostBack)
            {
                WebAccessCodeTextBox.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('BtnGetResult').click();return false;}} else {return true}; ");
            }
        }

        protected void WebAccessCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ErrorLabel.Text = null;
            this.StatusLabel.Text = null; 
            this.PanelResult.Visible = false;
            this.ErrorDownloadMainResultLabel.Text = null;
            this.ErrorDownloadMicroResultLabel.Text = null;
            this.ErrorDownloadExtraAttachmentResultLabel.Text = null;
            this.ResultPDFLabel.Text = GetLocalResourceObject("ResultPDFLabelText").ToString();
            this.ResultPDFMicroLabel.Text = GetLocalResourceObject("ResultPDFMicroLabelText").ToString();
            this.ResultPDFExtraAttachmentLabel.Text = GetLocalResourceObject("ResultPDFExtraAttachmentLabelText").ToString();
            this.ResultPDFExtraAttachmentLabel2.Text = GetLocalResourceObject("ResultPDFExtraAttachmentLabelText2").ToString();
            this.ResultPDFExtraAttachmentLabel3.Text = GetLocalResourceObject("ResultPDFExtraAttachmentLabelText3").ToString();
            this.ResultPDFExtraAttachmentLabel4.Text = GetLocalResourceObject("ResultPDFExtraAttachmentLabelText4").ToString();
            this.ResultPDFExtraAttachmentLabel5.Text = GetLocalResourceObject("ResultPDFExtraAttachmentLabelText5").ToString();
            this.tabPanelMainResult.Style["display"] = "none";
            this.DownloadResultMain.Style["display"] = "none";
            this.tabPanelMicroResult.Style["display"] = "none";
            this.DownloadResultMicro.Style["display"] = "none";
            this.tabPanelExtraAttachmentResult.Style["display"] = "none";
            this.DownloadResultExtraAttachment.Style["display"] = "none";
            this.tabPanelExtraAttachmentResult2.Style["display"] = "none";
            this.DownloadResultExtraAttachment2.Style["display"] = "none";
            this.tabPanelExtraAttachmentResult3.Style["display"] = "none";
            this.DownloadResultExtraAttachment3.Style["display"] = "none";
            this.tabPanelExtraAttachmentResult4.Style["display"] = "none";
            this.DownloadResultExtraAttachment4.Style["display"] = "none";
            this.tabPanelExtraAttachmentResult5.Style["display"] = "none";
            this.DownloadResultExtraAttachment5.Style["display"] = "none";
            this.rowResultAllLabel.Style["display"] = "none";

            if (string.IsNullOrEmpty(this.tabPanelMainResult.Attributes["class"])) { }
            else
            {
                var newClassValue = this.panelMainResult.Attributes["class"].Replace("tab-pane fade in active", "tab-pane fade");
                this.panelMainResult.Attributes["class"] = newClassValue;
            }
            if (string.IsNullOrEmpty(this.tabPanelMicroResult.Attributes["class"])) { }
            else
            {
                var newClassValue = this.panelMicroResult.Attributes["class"].Replace("tab-pane fade in active", "tab-pane fade");
                this.panelMicroResult.Attributes["class"] = newClassValue;
            }
            if (string.IsNullOrEmpty(this.tabPanelExtraAttachmentResult.Attributes["class"])) { }
            else
            {
                var newClassValue = this.panelExtraAttachmentResult.Attributes["class"].Replace("tab-pane fade in active", "tab-pane fade");
                this.panelExtraAttachmentResult.Attributes["class"] = newClassValue;
            }
        }

        protected void BtnGetResult_Click(object sender, EventArgs e)
        {
            ArrayList arrExtraAttachment = null; 
            bool flag = false; 
            bool active = true; 
            if (String.IsNullOrEmpty(WebAccessCodeTextBox.Text))
            {
                this.ErrorLabel.Text = GetLocalResourceObject("ErrorLabelText").ToString();
            }
            else
            {
                try
                {
                    ResultServiceClient client =  new ResultServiceClient();
                    string login = "", pass = "";
                    login = ConfigurationManager.AppSettings["loginForService"];
                    pass = ConfigurationManager.AppSettings["passwordForService"];

                    client.ClientCredentials.Windows.ClientCredential.UserName = login;
                    client.ClientCredentials.Windows.ClientCredential.Password = pass;

                    var resultSummary = client.GetResultSummaryByWebCode(WebAccessCodeTextBox.Text);
                    this.logger.Trace("CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text + " | IP-адрес: " + HttpContext.Current.Request.UserHostAddress);
                    if (resultSummary == null)
                    {
                        this.StatusLabel.ForeColor = System.Drawing.Color.Red;
                        this.StatusLabel.Text = GetLocalResourceObject("StatusLabelOrderIsNotFound").ToString();
                    }
                    else
                    {
                        this.StatusLabel.Text = null;
                        if ((resultSummary.Results.Count() > 0) && (resultSummary.IsFinal == true)) // ваш заказ готов результаты есть
                        {
                            this.StatusLabel.ForeColor = System.Drawing.Color.Green;
                            this.StatusLabel.Text = GetLocalResourceObject("StatusLabelReady").ToString();                            
                        }
                        if ((resultSummary.Results.Count() > 0) && (resultSummary.IsFinal == false)) // ваш заказ не готов показаны предварительные результаты
                        {
                            this.StatusLabel.ForeColor = System.Drawing.Color.Red;
                            this.StatusLabel.Text = GetLocalResourceObject("StatusLabelNotReady").ToString();                           
                        }
                        if ((resultSummary.Results.Count() == 0) && (resultSummary.IsFinal == true)) // по вашему заказу нет результатов, заказ закрыт
                        {
                            this.StatusLabel.ForeColor = System.Drawing.Color.Red;
                            this.StatusLabel.Text = GetLocalResourceObject("StatusLabelPerezabor").ToString();
                            logger.Error(this.StatusLabel.Text + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                            return;
                        }
                        if ((resultSummary.Results.Count() == 0) && (resultSummary.IsFinal == false)) // по вашему заказу нет результатов
                        {
                            this.StatusLabel.ForeColor = System.Drawing.Color.Red;
                            this.StatusLabel.Text = GetLocalResourceObject("StatusLabelNotResult").ToString();
                            return;
                        }
                        this.PanelResult.Visible = true;
                        Session.RemoveAll();
                        Session["barcode"] = resultSummary.Barcode.ToString();
                        int countExtraAttachment = 1;
                        foreach (var result in resultSummary.Results)
                        {
                            if (String.Equals("SilabMainResult", result.Type.ToString()))
                            {                                
                                Session["SilabMainResultUri"] = result.ServiceUri;                           
                                this.tabPanelMainResult.Style["display"] = "inline-block";
                                this.DownloadResultMain.Style["display"] = "inline-block";

                                if (string.IsNullOrEmpty(this.tabPanelMainResult.Attributes["class"]))
                                {

                                }
                                else
                                {
                                    var newClassValue = this.panelMainResult.Attributes["class"].Replace("tab-pane fade in active", "tab-pane fade");
                                    this.panelMainResult.Attributes["class"] = newClassValue;
                                    this.tabPanelMainResult.Attributes.Remove("class");
                                }
                                if (active)
                                {
                                    var newClassValue = this.panelMainResult.Attributes["class"].Replace("tab-pane fade", "tab-pane fade in active");
                                    this.panelMainResult.Attributes["class"] = newClassValue;
                                    this.tabPanelMainResult.Attributes.Add("class", "active");
                                    active = false;
                                }
                               
                            }
                            else
                            if (String.Equals("SilabMicroResult", result.Type.ToString()))
                            {
                                Session["SilabMicroResultUri"] = result.ServiceUri;
                                this.tabPanelMicroResult.Style["display"] = "inline-block";
                                this.DownloadResultMicro.Style["display"] = "inline-block";
                                //this.panelMicroResult.Style["display"] = "block";

                                if (string.IsNullOrEmpty(this.tabPanelMicroResult.Attributes["class"]))
                                {

                                }
                                else
                                {
                                    var newClassValue = this.panelMicroResult.Attributes["class"].Replace("tab-pane fade in active", "tab-pane fade");
                                    this.panelMicroResult.Attributes["class"] = newClassValue;
                                    this.tabPanelMicroResult.Attributes.Remove("class");
                                }
                                if (active)
                                {
                                    var newClassValue = this.panelMicroResult.Attributes["class"].Replace("tab-pane fade", "tab-pane fade in active");
                                    this.panelMicroResult.Attributes["class"] = newClassValue;
                                    this.tabPanelMicroResult.Attributes.Add("class", "active");
                                    active = false;
                                }                               
                            }
                            else
                            if (String.Equals("ExtraAttachment", result.Type.ToString()))
                            {
                                if (flag == false)
                                {
                                    arrExtraAttachment = new ArrayList();
                                }
                                flag = true;
                                arrExtraAttachment.Add(result.ServiceUri);
                                if (countExtraAttachment == 1)
                                {
                                    this.tabPanelExtraAttachmentResult.Style["display"] = "inline-block";
                                    this.DownloadResultExtraAttachment.Style["display"] = "inline-block";
                                }
                                if (countExtraAttachment == 2)
                                {
                                    this.tabPanelExtraAttachmentResult2.Style["display"] = "inline-block";
                                    this.DownloadResultExtraAttachment2.Style["display"] = "inline-block";
                                }
                                if (countExtraAttachment == 3)
                                {
                                    this.tabPanelExtraAttachmentResult3.Style["display"] = "inline-block";
                                    this.DownloadResultExtraAttachment3.Style["display"] = "inline-block";
                                }
                                if (countExtraAttachment == 4)
                                {
                                    this.tabPanelExtraAttachmentResult4.Style["display"] = "inline-block";
                                    this.DownloadResultExtraAttachment4.Style["display"] = "inline-block";
                                }
                                if (countExtraAttachment == 5)
                                {
                                    this.tabPanelExtraAttachmentResult5.Style["display"] = "inline-block";
                                    this.DownloadResultExtraAttachment5.Style["display"] = "inline-block";
                                }
                                countExtraAttachment++;

                                if (string.IsNullOrEmpty(this.tabPanelExtraAttachmentResult.Attributes["class"]))
                                {

                                }
                                else
                                {
                                    var newClassValue = this.panelExtraAttachmentResult.Attributes["class"].Replace("tab-pane fade in active", "tab-pane fade");
                                    this.panelExtraAttachmentResult.Attributes["class"] = newClassValue;
                                    this.tabPanelExtraAttachmentResult.Attributes.Remove("class");
                                }
                                if (active)
                                {
                                    var newClassValue = this.panelExtraAttachmentResult.Attributes["class"].Replace("tab-pane fade", "tab-pane fade in active");
                                    this.panelExtraAttachmentResult.Attributes["class"] = newClassValue;                                   
                                    this.tabPanelExtraAttachmentResult.Attributes.Add("class", "active");
                                    active = false;
                                }
                            }
                        }
                        if (flag)
                        {
                            Session["ExtraAttachmentUri"] = arrExtraAttachment;
                        }
                        if (resultSummary.Results.Length >= 2)
                        {
                            this.ResultAll.Text = GetLocalResourceObject("ResultAllText").ToString();
                            this.rowResultAllLabel.Style["display"] = "block";
                        }
                    }
                    client.Close();
                }
                catch (Exception ex)
                {
                    this.StatusLabel.ForeColor = System.Drawing.Color.Red;
                    this.StatusLabel.Text = GetLocalResourceObject("StatusLabelExeption").ToString(); 
                    logger.Error(ex.Message +" | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
            }
        }

        private void WarningMessageCulture()
        {
            if (!String.Equals(UICulture, "Ukrainian"))
                this.WarningMessage.Visible = true;
        }

        protected void MainExportButton_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                WebClient webClient = new WebClient();
                //webClient.Credentials = CredentialCache.DefaultCredentials;
                string login = "", pass = "";
                login = ConfigurationManager.AppSettings["loginForService"];
                pass = ConfigurationManager.AppSettings["passwordForService"];
                webClient.Credentials = new NetworkCredential(login, pass);
                byte[] arr = webClient.DownloadData(Session["SilabMainResultUri"].ToString());

                Response.Clear();
                Response.AppendHeader("Content-Disposition", "attachment; filename=SynevoResults" + Session["barcode"] + ".pdf");
                Response.ContentType = "application/pdf";
                Response.OutputStream.Write(arr, 0, arr.Length);
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            }
            catch (HttpException ex)
            {
                logger.Info(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            }
        }

        protected void MicroExportButton_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                WebClient webClient = new WebClient();
                string login = "", pass = "";
                login = ConfigurationManager.AppSettings["loginForService"];
                pass = ConfigurationManager.AppSettings["passwordForService"];
                webClient.Credentials = new NetworkCredential(login, pass);
                byte[] arr = webClient.DownloadData(Session["SilabMicroResultUri"].ToString());

                Response.Clear();
                Response.AppendHeader("Content-Disposition", "attachment; filename=SynevoResults" + Session["barcode"] + "_Micro.pdf");
                Response.ContentType = "application/pdf";
                Response.OutputStream.Write(arr, 0, arr.Length);
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            }
            catch(HttpException ex)
            {
                logger.Info(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            }
        }

        protected void ExtraAttachmentExportButton_Click(object sender, ImageClickEventArgs e)
        {
            ArrayList arrExtraAttachment = (ArrayList)Session["ExtraAttachmentUri"];
            WebClient webClient = new WebClient();
            string login = "", pass = "";
            login = ConfigurationManager.AppSettings["loginForService"];
            pass = ConfigurationManager.AppSettings["passwordForService"];
            webClient.Credentials = new NetworkCredential(login, pass);
            try
            {
                string tmp = arrExtraAttachment[0].ToString();
                byte[] arr = webClient.DownloadData(arrExtraAttachment[0].ToString());

                Response.Clear();
                Response.AppendHeader("Content-Disposition", "attachment; filename=SynevoAddResults" + Session["barcode"] + "_Add.pdf");
                Response.ContentType = "application/pdf";
                Response.OutputStream.Write(arr, 0, arr.Length);
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            }
            catch (HttpException ex)
            {
                logger.Info(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            }
            //else
            //{
            //    string zipFileName = null;
            //    try
            //    {
            //        DirectoryInfo directoryinfo = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("en", "en")) + " SynevoAddResults" + Session["barcode"]);

            //        string fileNameAdd = "SynevoAddResults" + Session["barcode"] + "_";
            //        zipFileName = directoryinfo.FullName + ".zip";

            //        for (int i=0; i< arrExtraAttachment.Count; i++)
            //        {
            //            webClient.DownloadFile(arrExtraAttachment[i].ToString(), Path.Combine(directoryinfo.FullName, fileNameAdd + i + ".pdf"));
            //        }
                   

            //        ZipFile.CreateFromDirectory(directoryinfo.FullName, zipFileName);
            //        directoryinfo.Delete(true);

            //        Response.Clear();
            //        Response.AppendHeader("Content-Disposition", "attachment; filename=" + directoryinfo.Name + ".zip");
            //        Response.ContentType = "application/zip";
            //        Response.WriteFile(zipFileName);
            //        Response.Flush();
            //        Response.SuppressContent = true;
            //        HttpContext.Current.ApplicationInstance.CompleteRequest();
            //    }
            //    catch (System.Threading.ThreadAbortException ex)
            //    {
            //        logger.Error(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.Error(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
            //    }
            //    finally
            //    {
            //        if (File.Exists(zipFileName))
            //        {
            //            File.Delete(zipFileName);
            //        }
            //    }
            //}
        }

        protected void ExtraAttachmentExportButton2_Click(object sender, ImageClickEventArgs e)
        {
            ArrayList arrExtraAttachment = (ArrayList)Session["ExtraAttachmentUri"];
            WebClient webClient = new WebClient();
            string login = "", pass = "";
            login = ConfigurationManager.AppSettings["loginForService"];
            pass = ConfigurationManager.AppSettings["passwordForService"];
            webClient.Credentials = new NetworkCredential(login, pass);
            if (arrExtraAttachment.Count == 2) // 
            {
                try
                {
                    string tmp = arrExtraAttachment[1].ToString();
                    byte[] arr = webClient.DownloadData(arrExtraAttachment[1].ToString());

                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "attachment; filename=SynevoAddResults" + Session["barcode"] + "_Add2.pdf");
                    Response.ContentType = "application/pdf";
                    Response.OutputStream.Write(arr, 0, arr.Length);
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
                catch (HttpException ex)
                {
                    logger.Info(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
            }
        }

        protected void ExtraAttachmentExportButton3_Click(object sender, ImageClickEventArgs e)
        {
            ArrayList arrExtraAttachment = (ArrayList)Session["ExtraAttachmentUri"];
            WebClient webClient = new WebClient();
            string login = "", pass = "";
            login = ConfigurationManager.AppSettings["loginForService"];
            pass = ConfigurationManager.AppSettings["passwordForService"];
            webClient.Credentials = new NetworkCredential(login, pass);
            if (arrExtraAttachment.Count == 3) // 
            {
                try
                {
                    string tmp = arrExtraAttachment[2].ToString();
                    byte[] arr = webClient.DownloadData(arrExtraAttachment[2].ToString());

                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "attachment; filename=SynevoAddResults" + Session["barcode"] + "_Add3.pdf");
                    Response.ContentType = "application/pdf";
                    Response.OutputStream.Write(arr, 0, arr.Length);
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
                catch (HttpException ex)
                {
                    logger.Info(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
            }
        }

        protected void ExtraAttachmentExportButton4_Click(object sender, ImageClickEventArgs e)
        {
            ArrayList arrExtraAttachment = (ArrayList)Session["ExtraAttachmentUri"];
            WebClient webClient = new WebClient();
            string login = "", pass = "";
            login = ConfigurationManager.AppSettings["loginForService"];
            pass = ConfigurationManager.AppSettings["passwordForService"];
            webClient.Credentials = new NetworkCredential(login, pass);
            if (arrExtraAttachment.Count == 4) // 
            {
                try
                {
                    string tmp = arrExtraAttachment[3].ToString();
                    byte[] arr = webClient.DownloadData(arrExtraAttachment[3].ToString());

                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "attachment; filename=SynevoAddResults" + Session["barcode"] + "_Add4.pdf");
                    Response.ContentType = "application/pdf";
                    Response.OutputStream.Write(arr, 0, arr.Length);
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
                catch (HttpException ex)
                {
                    logger.Info(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
            }
        }

        protected void ExtraAttachmentExportButton5_Click(object sender, ImageClickEventArgs e)
        {
            ArrayList arrExtraAttachment = (ArrayList)Session["ExtraAttachmentUri"];
            WebClient webClient = new WebClient();
            string login = "", pass = "";
            login = ConfigurationManager.AppSettings["loginForService"];
            pass = ConfigurationManager.AppSettings["passwordForService"];
            webClient.Credentials = new NetworkCredential(login, pass);
            if (arrExtraAttachment.Count == 5) // 
            {
                try
                {
                    string tmp = arrExtraAttachment[4].ToString();
                    byte[] arr = webClient.DownloadData(arrExtraAttachment[4].ToString());

                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "attachment; filename=SynevoAddResults" + Session["barcode"] + "_Add5.pdf");
                    Response.ContentType = "application/pdf";
                    Response.OutputStream.Write(arr, 0, arr.Length);
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
                catch (HttpException ex)
                {
                    logger.Info(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
            }
        }

        protected void ResultAllExportButton_Click(object sender, ImageClickEventArgs e)
        {
            string zipFileName = null;
            DirectoryInfo directoryinfo = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("en", "en")) + " SynevoResults" + Session["barcode"]);

            try
            {              
                WebClient webClient = new WebClient();
                string login = "", pass = "";
                login = ConfigurationManager.AppSettings["loginForService"];
                pass = ConfigurationManager.AppSettings["passwordForService"];
                webClient.Credentials = new NetworkCredential(login, pass);

                string fileNameMain = "SynevoResults" + Session["barcode"] + ".pdf";
                string fileNameMicro = "SynevoResults" + Session["barcode"] + "_Micro.pdf";
                string fileNameAdd = "SynevoAddResults" + Session["barcode"] + "_";
                zipFileName = directoryinfo.FullName + ".zip";

                try
                {
                    if (Session["SilabMainResultUri"] != null)
                    {
                        webClient.DownloadFile(Session["SilabMainResultUri"].ToString(), Path.Combine(directoryinfo.FullName, fileNameMain));
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }
                try
                {
                    if (Session["SilabMicroResultUri"] != null)
                    {
                        webClient.DownloadFile(Session["SilabMicroResultUri"].ToString(), Path.Combine(directoryinfo.FullName, fileNameMicro));
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }

                try
                {
                    if (Session["ExtraAttachmentUri"] != null)
                    {
                        ArrayList arrExtraAttachment = (ArrayList)Session["ExtraAttachmentUri"];
                        for (int i = 0; i < arrExtraAttachment.Count; i++)
                        {
                            webClient.DownloadFile(arrExtraAttachment[i].ToString(), Path.Combine(directoryinfo.FullName, fileNameAdd + i + ".pdf"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                }

                ZipFile.CreateFromDirectory(directoryinfo.FullName, zipFileName);
                directoryinfo.Delete(true);

                Response.Clear();
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + directoryinfo.Name + ".zip");
                Response.ContentType = "application/zip";
                Response.WriteFile(zipFileName);
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                this.StatusLabel.ForeColor = System.Drawing.Color.Red;
                this.StatusLabel.Text = GetLocalResourceObject("StatusLabelExeption").ToString();
                logger.Error(ex.Message + " | Row: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + " | CodeForWebFFSOrder: " + WebAccessCodeTextBox.Text);
                directoryinfo.Delete(true);
            }
            finally
            {
                if (File.Exists(zipFileName))
                {
                    File.Delete(zipFileName);
                }
            }
        }

        protected void BtnClearSession_Click(object sender, ImageClickEventArgs e)
        {
            this.WebAccessCodeTextBox.Text = null;
            this.ErrorLabel.Text = null;
            this.StatusLabel.Text = null;
            this.PanelResult.Visible = false;
            Session.RemoveAll();
        }
    }
}