<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="WebReport.Report"%> 

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>
        <asp:Literal ID="myLiteral" runat="server" Text="<%$ Resources:Title %>"></asp:Literal>
    </title>
    <%-- include Bootstrap --%>        
    <script src="../Scripts/jquery-1.9.1.js"></script>
    <script src="../Scripts/jquery-ui.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <link href="Content/bootstrap.min.css" rel="stylesheet" /> 
    <link href="css/Style.css"  type = "text/css" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server">
<div class="container">
<div class="row">  
    <div class="col-xs-12">
    <asp:Image ID="Image1" runat="server" ImageUrl="img/Logo_Jelt.png" Width="130" Height="140" /><br />
        <asp:ImageButton ID="ImageButtonClearSession" runat="server" 
            ImageUrl="img/edit-clear.png"  
            ToolTip="<%$ Resources:ImageButtonClearSession %>"
            OnClick="BtnClearSession_Click" />
    </div>  
</div>                 
<div class="row">
    <div class="col-xs-10" style="padding-left:40px; margin-top:-60px;"> 
        <asp:Label ID="Label1" runat="server" Text="<%$ Resources:LabelHeader %>"></asp:Label><br /><br /> 
    </div>          
</div> 
<div class="col-xs-12" style="text-align:center;">  
        <asp:Label ID="WarningMessage" runat="server" Text="<%$ Resources:WarningMessage %>" Visible="false"></asp:Label> 
</div>
<div class="row">
<div class="col-xs-12" style="padding-left:40px">
    <asp:Label ID="Label2" runat="server" Text="<%$ Resources:LabelCode %>"></asp:Label>
    </div>  
</div> 
<div class="row">
<div class="col-xs-4" style="padding-left:40px">
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://www.synevo.ua/uk/kod-dostupa" Target="_blank"><i id="ex-on-img"><span>
            <asp:Label ID="Label3" runat="server" Text ="<%$ Resources:LabelExample %>"></asp:Label></span></i></asp:HyperLink>             
    </div> 
</div> 

<div class="row">
<div class="col-xs-12" style="padding-top:0px; padding-left:40px">
        <asp:TextBox ID="WebAccessCodeTextBox" runat="server" MaxLength="20"  OnTextChanged="WebAccessCodeTextBox_TextChanged"></asp:TextBox>
    </div>     
</div> 


<div class="row">
<div class="col-xs-12" style="padding-top:0px; padding-left:40px">
        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label>
    </div>     
</div> 

<div class="row" 
    style="padding: 5px 0px 2px 7px; 
    text-align:center; 
    max-width:120px;
    min-width:120px;
    margin:auto;
    border-radius: 8px;
    text-transform: uppercase;
    font-weight: bold;
    font-family: Helvetica, Arial, sans-serif;
    background-color: #ffb824;
    background: linear-gradient(to top, #faba01, #ffe696);
    border: 1px solid #ccc;
    box-shadow: 0 0 5px rgba(0,0,0,0.3); 
    min-width: 305px;">  
        <asp:Button ID="BtnGetResult" runat="server" Text="<%$ Resources:BtnGetResultText %>" ToolTip="<%$ Resources:BtnGetResultToolTip %>" BackColor="#FCB829" OnClick="BtnGetResult_Click"/>          
</div> 

<div class="row" style="padding-top:10px;">
    <div class="col-xs-" style="padding-top:0px;
            text-align:left;
            max-width:270px; 
            min-width:270px;
            margin:auto;">
        <asp:Label ID="StatusLabel" runat="server"></asp:Label>   
    </div>
</div>

<asp:Panel ID="PanelResult" runat="server" BackColor="White" BorderStyle="None" 
        HorizontalAlign="Left" Width="100%" Visible="False">   

    <asp:Image ID="ImageGif" runat="server" ImageUrl="img/712.gif" />
    <div class="row" style="padding-top:30px;"> </div>
    <div class="row" id="rowResultAllLabel" style="margin-top:10px; display:none;" runat="server">
        <div class="col-xs-12"
            style="min-width:305px; text-align:center;">
            <asp:Label ID="ResultAll" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label>
            <asp:ImageButton ID="ResultAllExportButton" runat="server" 
                ImageUrl="img/zip_file.png" onclick="ResultAllExportButton_Click" 
                ToolTip="<%$ Resources:ButtonDownloadToolTip %>" />&nbsp
        </div>        
    </div> 
 
           
         
    <ul class="nav nav-tabs" style="margin-top:30px">
        <li id="tabPanelMainResult" runat="server">
            <a data-toggle="tab" href="#panelMainResult" style="text-align: left; padding-right: 40px;"><asp:Label ID="ResultPDFLabel" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label></a>
        </li>
    <li id="DownloadResultMain" runat="server">
            <asp:ImageButton ID="MainExportButton" runat="server" 
            ImageUrl="img/downloading.png"
            onclick="MainExportButton_Click"
            ToolTip="<%$ Resources:ButtonToolTip %>" />
        </li>
        <li id="tabPanelMicroResult" runat="server">
            <a data-toggle="tab" href="#panelMicroResult" style="text-align: left; padding-right: 40px;"><asp:Label ID="ResultPDFMicroLabel" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label></a>
        </li>
        <li id="DownloadResultMicro" runat="server">
            <asp:ImageButton ID="MicroExportButton" runat="server" 
            ImageUrl="img/downloading.png" 
            onclick="MicroExportButton_Click"
            ToolTip="<%$ Resources:ButtonToolTip %>" />
        </li>
        <li id="tabPanelExtraAttachmentResult" runat="server">
            <a data-toggle="tab" href="#panelExtraAttachmentResult" style="text-align: left; padding-right: 40px;"><asp:Label ID="ResultPDFExtraAttachmentLabel" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label></a>
        </li>
        <li id="DownloadResultExtraAttachment" runat="server">
            <asp:ImageButton ID="ExtraAttachmentExportButton" runat="server" 
            ImageUrl="img/downloading.png"
            onclick="ExtraAttachmentExportButton_Click"
            ToolTip="<%$ Resources:ButtonToolTip %>" />
        </li>
        <li id="tabPanelExtraAttachmentResult2" runat="server">
            <a data-toggle="tab" href="#panelExtraAttachmentResult2" style="text-align: left; padding-right: 40px;"><asp:Label ID="ResultPDFExtraAttachmentLabel2" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label></a>
        </li>
        <li id="DownloadResultExtraAttachment2" runat="server">
            <asp:ImageButton ID="ExtraAttachmentExportButton2" runat="server" 
            ImageUrl="img/downloading.png"
            onclick="ExtraAttachmentExportButton2_Click"
            ToolTip="<%$ Resources:ButtonToolTip %>" />
        </li>
        <li id="tabPanelExtraAttachmentResult3" runat="server">
            <a data-toggle="tab" href="#panelExtraAttachmentResult3" style="text-align: left; padding-right: 40px;"><asp:Label ID="ResultPDFExtraAttachmentLabel3" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label></a>
        </li>
        <li id="DownloadResultExtraAttachment3" runat="server">
            <asp:ImageButton ID="ExtraAttachmentExportButton3" runat="server" 
            ImageUrl="img/downloading.png"
            onclick="ExtraAttachmentExportButton3_Click"
            ToolTip="<%$ Resources:ButtonToolTip %>" />
        </li>
        <li id="tabPanelExtraAttachmentResult4" runat="server">
            <a data-toggle="tab" href="#panelExtraAttachmentResult4" style="text-align: left; padding-right: 40px;"><asp:Label ID="ResultPDFExtraAttachmentLabel4" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label></a>
        </li>
        <li id="DownloadResultExtraAttachment4" runat="server">
            <asp:ImageButton ID="ExtraAttachmentExportButton4" runat="server" 
            ImageUrl="img/downloading.png"
            onclick="ExtraAttachmentExportButton4_Click"
            ToolTip="<%$ Resources:ButtonToolTip %>" />
        </li>
        <li id="tabPanelExtraAttachmentResult5" runat="server">
            <a data-toggle="tab" href="#panelExtraAttachmentResult5" style="text-align: left; padding-right: 40px;"><asp:Label ID="ResultPDFExtraAttachmentLabel5" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label></a>
        </li>
        <li id="DownloadResultExtraAttachment5" runat="server">
            <asp:ImageButton ID="ExtraAttachmentExportButton5" runat="server" 
            ImageUrl="img/downloading.png"
            onclick="ExtraAttachmentExportButton5_Click"
            ToolTip="<%$ Resources:ButtonToolTip %>" />
        </li>
    </ul>     
    <br />
         
    <div class="row" style="text-align:center;"> 
    <div class="tab-content">
        <div id="panelMainResult" class="tab-pane fade" runat="server">
            <%-- рендеринг MainResult --%>
            <asp:Label ID="ErrorDownloadMainResultLabel" runat="server"></asp:Label>   
            <%--<iframe id="MainIframe" src="" width="780px" height="1050px" frameborder="0" webkitAllowFullScreen mozallowfullscreen allowFullScreen></iframe> --%>
            <object data="ResultHandler?Main=Main" type="application/pdf" width="780px" height="1050px" ></object>
        </div>

        <div id="panelMicroResult" class="tab-pane fade" runat="server">
                <%-- рендеринг MicroResult --%>
            <br /> 
            <asp:Label ID="ErrorDownloadMicroResultLabel" runat="server"></asp:Label>
            <object data="ResultHandler?Micro=Micro" type="application/pdf" width="780px" height="1050px" ></object>  
        </div>

        <div id="panelExtraAttachmentResult" class="tab-pane fade" runat="server">
            <%-- рендеринг ExtraAttachment --%>
            <asp:Label ID="ErrorDownloadExtraAttachmentResultLabel" runat="server"></asp:Label>
            <object data="ResultHandler?ExtraAttachment=ExtraAttachment" type="application/pdf" width="780px" height="1050px" ></object>
        </div> 
        <div id="panelExtraAttachmentResult2" class="tab-pane fade" runat="server">
            <object data="ResultHandler?ExtraAttachment=ExtraAttachment2" type="application/pdf" width="780px" height="1050px" ></object>
        </div> 
        <div id="panelExtraAttachmentResult3" class="tab-pane fade" runat="server">
            <object data="ResultHandler?ExtraAttachment=ExtraAttachment3" type="application/pdf" width="780px" height="1050px" ></object>
        </div> 
        <div id="panelExtraAttachmentResult4" class="tab-pane fade" runat="server">
            <object data="ResultHandler?ExtraAttachment=ExtraAttachment4" type="application/pdf" width="780px" height="1050px" ></object>
        </div> 
        <div id="panelExtraAttachmentResult5" class="tab-pane fade" runat="server">
            <object data="ResultHandler?ExtraAttachment=ExtraAttachment5" type="application/pdf" width="780px" height="1050px" ></object>
        </div> 

    </div> 
    </div>      
</asp:Panel>
    <script>
        setTimeout('document.getElementById("ImageGif").style.display = "none"',2000);
        
    </script> 
</div>
   </form>
</body>
</html>
