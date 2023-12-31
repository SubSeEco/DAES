﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.17929
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// Este código fuente fue generado automáticamente por wsdl, Versión=4.0.30319.1.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name="AgileSignerFacadeWSSoap11Binding", Namespace="http://signer.agile.cl")]
public partial class AgileSignerFacadeWS : System.Web.Services.Protocols.SoapHttpClientProtocol {
    
    private System.Threading.SendOrPostCallback signPDFp12OperationCompleted;
    
    private System.Threading.SendOrPostCallback signPDFOperationCompleted;
    
    private System.Threading.SendOrPostCallback signXMLOperationCompleted;
    
    private System.Threading.SendOrPostCallback setPDFPasswordOperationCompleted;
    
    private System.Threading.SendOrPostCallback signPDFServerSideOperationCompleted;
    
    private System.Threading.SendOrPostCallback signXMLRefsOperationCompleted;
    
    private System.Threading.SendOrPostCallback validateXMLOperationCompleted;
    
    /// <remarks/>
    public AgileSignerFacadeWS() {
        this.Url = "http://spencer.economia.cl:8080/AgileSigner26WSLuna5/services/AgileSignerFacadeWS" +
            ".AgileSignerFacadeWSHttpSoap11Endpoint/";
    }
    
    /// <remarks/>
    public event signPDFp12CompletedEventHandler signPDFp12Completed;
    
    /// <remarks/>
    public event signPDFCompletedEventHandler signPDFCompleted;
    
    /// <remarks/>
    public event signXMLCompletedEventHandler signXMLCompleted;
    
    /// <remarks/>
    public event setPDFPasswordCompletedEventHandler setPDFPasswordCompleted;
    
    /// <remarks/>
    public event signPDFServerSideCompletedEventHandler signPDFServerSideCompleted;
    
    /// <remarks/>
    public event signXMLRefsCompletedEventHandler signXMLRefsCompleted;
    
    /// <remarks/>
    public event validateXMLCompletedEventHandler validateXMLCompleted;
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:signPDFp12", RequestNamespace="http://signer.agile.cl", ResponseNamespace="http://signer.agile.cl", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("return", DataType="base64Binary", IsNullable=true)]
    public byte[] signPDFp12([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", IsNullable=true)] byte[] certP12, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", IsNullable=true)] byte[] pdfByte, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string certPass, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string location, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string comment, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string nombreAplicacion, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string informacionAplicacion, bool valida, [System.Xml.Serialization.XmlIgnoreAttribute()] bool validaSpecified) {
        object[] results = this.Invoke("signPDFp12", new object[] {
                    certP12,
                    pdfByte,
                    certPass,
                    location,
                    comment,
                    nombreAplicacion,
                    informacionAplicacion,
                    valida,
                    validaSpecified});
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginsignPDFp12(byte[] certP12, byte[] pdfByte, string certPass, string location, string comment, string nombreAplicacion, string informacionAplicacion, bool valida, bool validaSpecified, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("signPDFp12", new object[] {
                    certP12,
                    pdfByte,
                    certPass,
                    location,
                    comment,
                    nombreAplicacion,
                    informacionAplicacion,
                    valida,
                    validaSpecified}, callback, asyncState);
    }
    
    /// <remarks/>
    public byte[] EndsignPDFp12(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public void signPDFp12Async(byte[] certP12, byte[] pdfByte, string certPass, string location, string comment, string nombreAplicacion, string informacionAplicacion, bool valida, bool validaSpecified) {
        this.signPDFp12Async(certP12, pdfByte, certPass, location, comment, nombreAplicacion, informacionAplicacion, valida, validaSpecified, null);
    }
    
    /// <remarks/>
    public void signPDFp12Async(byte[] certP12, byte[] pdfByte, string certPass, string location, string comment, string nombreAplicacion, string informacionAplicacion, bool valida, bool validaSpecified, object userState) {
        if ((this.signPDFp12OperationCompleted == null)) {
            this.signPDFp12OperationCompleted = new System.Threading.SendOrPostCallback(this.OnsignPDFp12OperationCompleted);
        }
        this.InvokeAsync("signPDFp12", new object[] {
                    certP12,
                    pdfByte,
                    certPass,
                    location,
                    comment,
                    nombreAplicacion,
                    informacionAplicacion,
                    valida,
                    validaSpecified}, this.signPDFp12OperationCompleted, userState);
    }
    
    private void OnsignPDFp12OperationCompleted(object arg) {
        if ((this.signPDFp12Completed != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.signPDFp12Completed(this, new signPDFp12CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:signPDF", RequestNamespace="http://signer.agile.cl", ResponseNamespace="http://signer.agile.cl", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("return", DataType="base64Binary", IsNullable=true)]
    public byte[] signPDF([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string idServicio, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", IsNullable=true)] byte[] pdfByte, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string location, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string comment, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string nombreAplicacion, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string informacionAplicacion, bool valida, [System.Xml.Serialization.XmlIgnoreAttribute()] bool validaSpecified) {
        object[] results = this.Invoke("signPDF", new object[] {
                    idServicio,
                    pdfByte,
                    location,
                    comment,
                    nombreAplicacion,
                    informacionAplicacion,
                    valida,
                    validaSpecified});
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginsignPDF(string idServicio, byte[] pdfByte, string location, string comment, string nombreAplicacion, string informacionAplicacion, bool valida, bool validaSpecified, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("signPDF", new object[] {
                    idServicio,
                    pdfByte,
                    location,
                    comment,
                    nombreAplicacion,
                    informacionAplicacion,
                    valida,
                    validaSpecified}, callback, asyncState);
    }
    
    /// <remarks/>
    public byte[] EndsignPDF(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public void signPDFAsync(string idServicio, byte[] pdfByte, string location, string comment, string nombreAplicacion, string informacionAplicacion, bool valida, bool validaSpecified) {
        this.signPDFAsync(idServicio, pdfByte, location, comment, nombreAplicacion, informacionAplicacion, valida, validaSpecified, null);
    }
    
    /// <remarks/>
    public void signPDFAsync(string idServicio, byte[] pdfByte, string location, string comment, string nombreAplicacion, string informacionAplicacion, bool valida, bool validaSpecified, object userState) {
        if ((this.signPDFOperationCompleted == null)) {
            this.signPDFOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsignPDFOperationCompleted);
        }
        this.InvokeAsync("signPDF", new object[] {
                    idServicio,
                    pdfByte,
                    location,
                    comment,
                    nombreAplicacion,
                    informacionAplicacion,
                    valida,
                    validaSpecified}, this.signPDFOperationCompleted, userState);
    }
    
    private void OnsignPDFOperationCompleted(object arg) {
        if ((this.signPDFCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.signPDFCompleted(this, new signPDFCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:signXML", RequestNamespace="http://signer.agile.cl", ResponseNamespace="http://signer.agile.cl", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("return", DataType="base64Binary", IsNullable=true)]
    public byte[] signXML([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string idServicio, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", IsNullable=true)] byte[] xmlByte, bool valida, [System.Xml.Serialization.XmlIgnoreAttribute()] bool validaSpecified) {
        object[] results = this.Invoke("signXML", new object[] {
                    idServicio,
                    xmlByte,
                    valida,
                    validaSpecified});
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginsignXML(string idServicio, byte[] xmlByte, bool valida, bool validaSpecified, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("signXML", new object[] {
                    idServicio,
                    xmlByte,
                    valida,
                    validaSpecified}, callback, asyncState);
    }
    
    /// <remarks/>
    public byte[] EndsignXML(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public void signXMLAsync(string idServicio, byte[] xmlByte, bool valida, bool validaSpecified) {
        this.signXMLAsync(idServicio, xmlByte, valida, validaSpecified, null);
    }
    
    /// <remarks/>
    public void signXMLAsync(string idServicio, byte[] xmlByte, bool valida, bool validaSpecified, object userState) {
        if ((this.signXMLOperationCompleted == null)) {
            this.signXMLOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsignXMLOperationCompleted);
        }
        this.InvokeAsync("signXML", new object[] {
                    idServicio,
                    xmlByte,
                    valida,
                    validaSpecified}, this.signXMLOperationCompleted, userState);
    }
    
    private void OnsignXMLOperationCompleted(object arg) {
        if ((this.signXMLCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.signXMLCompleted(this, new signXMLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:setPDFPassword", RequestNamespace="http://signer.agile.cl", ResponseNamespace="http://signer.agile.cl", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("return", DataType="base64Binary", IsNullable=true)]
    public byte[] setPDFPassword([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", IsNullable=true)] byte[] pdfWithOutPassword, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string user, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string owner) {
        object[] results = this.Invoke("setPDFPassword", new object[] {
                    pdfWithOutPassword,
                    user,
                    owner});
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginsetPDFPassword(byte[] pdfWithOutPassword, string user, string owner, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("setPDFPassword", new object[] {
                    pdfWithOutPassword,
                    user,
                    owner}, callback, asyncState);
    }
    
    /// <remarks/>
    public byte[] EndsetPDFPassword(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public void setPDFPasswordAsync(byte[] pdfWithOutPassword, string user, string owner) {
        this.setPDFPasswordAsync(pdfWithOutPassword, user, owner, null);
    }
    
    /// <remarks/>
    public void setPDFPasswordAsync(byte[] pdfWithOutPassword, string user, string owner, object userState) {
        if ((this.setPDFPasswordOperationCompleted == null)) {
            this.setPDFPasswordOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsetPDFPasswordOperationCompleted);
        }
        this.InvokeAsync("setPDFPassword", new object[] {
                    pdfWithOutPassword,
                    user,
                    owner}, this.setPDFPasswordOperationCompleted, userState);
    }
    
    private void OnsetPDFPasswordOperationCompleted(object arg) {
        if ((this.setPDFPasswordCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.setPDFPasswordCompleted(this, new setPDFPasswordCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:signPDFServerSide", RequestNamespace="http://signer.agile.cl", ResponseNamespace="http://signer.agile.cl", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("return", IsNullable=true)]
    public string signPDFServerSide([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string idServicio, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string pdfFile, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string pdfSignedFile, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string location, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string comment, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string nombreAplicacion, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string informacionAplicacion, bool valida, [System.Xml.Serialization.XmlIgnoreAttribute()] bool validaSpecified) {
        object[] results = this.Invoke("signPDFServerSide", new object[] {
                    idServicio,
                    pdfFile,
                    pdfSignedFile,
                    location,
                    comment,
                    nombreAplicacion,
                    informacionAplicacion,
                    valida,
                    validaSpecified});
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginsignPDFServerSide(string idServicio, string pdfFile, string pdfSignedFile, string location, string comment, string nombreAplicacion, string informacionAplicacion, bool valida, bool validaSpecified, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("signPDFServerSide", new object[] {
                    idServicio,
                    pdfFile,
                    pdfSignedFile,
                    location,
                    comment,
                    nombreAplicacion,
                    informacionAplicacion,
                    valida,
                    validaSpecified}, callback, asyncState);
    }
    
    /// <remarks/>
    public string EndsignPDFServerSide(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public void signPDFServerSideAsync(string idServicio, string pdfFile, string pdfSignedFile, string location, string comment, string nombreAplicacion, string informacionAplicacion, bool valida, bool validaSpecified) {
        this.signPDFServerSideAsync(idServicio, pdfFile, pdfSignedFile, location, comment, nombreAplicacion, informacionAplicacion, valida, validaSpecified, null);
    }
    
    /// <remarks/>
    public void signPDFServerSideAsync(string idServicio, string pdfFile, string pdfSignedFile, string location, string comment, string nombreAplicacion, string informacionAplicacion, bool valida, bool validaSpecified, object userState) {
        if ((this.signPDFServerSideOperationCompleted == null)) {
            this.signPDFServerSideOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsignPDFServerSideOperationCompleted);
        }
        this.InvokeAsync("signPDFServerSide", new object[] {
                    idServicio,
                    pdfFile,
                    pdfSignedFile,
                    location,
                    comment,
                    nombreAplicacion,
                    informacionAplicacion,
                    valida,
                    validaSpecified}, this.signPDFServerSideOperationCompleted, userState);
    }
    
    private void OnsignPDFServerSideOperationCompleted(object arg) {
        if ((this.signPDFServerSideCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.signPDFServerSideCompleted(this, new signPDFServerSideCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:signXMLRefs", RequestNamespace="http://signer.agile.cl", ResponseNamespace="http://signer.agile.cl", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("return", DataType="base64Binary", IsNullable=true)]
    public byte[] signXMLRefs([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string idServicio, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", IsNullable=true)] byte[] xmlByte, [System.Xml.Serialization.XmlElementAttribute("referencias", IsNullable=true)] string[] referencias, bool valida, [System.Xml.Serialization.XmlIgnoreAttribute()] bool validaSpecified) {
        object[] results = this.Invoke("signXMLRefs", new object[] {
                    idServicio,
                    xmlByte,
                    referencias,
                    valida,
                    validaSpecified});
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginsignXMLRefs(string idServicio, byte[] xmlByte, string[] referencias, bool valida, bool validaSpecified, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("signXMLRefs", new object[] {
                    idServicio,
                    xmlByte,
                    referencias,
                    valida,
                    validaSpecified}, callback, asyncState);
    }
    
    /// <remarks/>
    public byte[] EndsignXMLRefs(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((byte[])(results[0]));
    }
    
    /// <remarks/>
    public void signXMLRefsAsync(string idServicio, byte[] xmlByte, string[] referencias, bool valida, bool validaSpecified) {
        this.signXMLRefsAsync(idServicio, xmlByte, referencias, valida, validaSpecified, null);
    }
    
    /// <remarks/>
    public void signXMLRefsAsync(string idServicio, byte[] xmlByte, string[] referencias, bool valida, bool validaSpecified, object userState) {
        if ((this.signXMLRefsOperationCompleted == null)) {
            this.signXMLRefsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsignXMLRefsOperationCompleted);
        }
        this.InvokeAsync("signXMLRefs", new object[] {
                    idServicio,
                    xmlByte,
                    referencias,
                    valida,
                    validaSpecified}, this.signXMLRefsOperationCompleted, userState);
    }
    
    private void OnsignXMLRefsOperationCompleted(object arg) {
        if ((this.signXMLRefsCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.signXMLRefsCompleted(this, new signXMLRefsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:validateXML", RequestNamespace="http://signer.agile.cl", ResponseNamespace="http://signer.agile.cl", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public void validateXML([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", IsNullable=true)] byte[] xmlBytes, bool validateCerts, [System.Xml.Serialization.XmlIgnoreAttribute()] bool validateCertsSpecified, out bool @return, [System.Xml.Serialization.XmlIgnoreAttribute()] out bool returnSpecified) {
        object[] results = this.Invoke("validateXML", new object[] {
                    xmlBytes,
                    validateCerts,
                    validateCertsSpecified});
        @return = ((bool)(results[0]));
        returnSpecified = ((bool)(results[1]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginvalidateXML(byte[] xmlBytes, bool validateCerts, bool validateCertsSpecified, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("validateXML", new object[] {
                    xmlBytes,
                    validateCerts,
                    validateCertsSpecified}, callback, asyncState);
    }
    
    /// <remarks/>
    public void EndvalidateXML(System.IAsyncResult asyncResult, out bool @return, out bool returnSpecified) {
        object[] results = this.EndInvoke(asyncResult);
        @return = ((bool)(results[0]));
        returnSpecified = ((bool)(results[1]));
    }
    
    /// <remarks/>
    public void validateXMLAsync(byte[] xmlBytes, bool validateCerts, bool validateCertsSpecified) {
        this.validateXMLAsync(xmlBytes, validateCerts, validateCertsSpecified, null);
    }
    
    /// <remarks/>
    public void validateXMLAsync(byte[] xmlBytes, bool validateCerts, bool validateCertsSpecified, object userState) {
        if ((this.validateXMLOperationCompleted == null)) {
            this.validateXMLOperationCompleted = new System.Threading.SendOrPostCallback(this.OnvalidateXMLOperationCompleted);
        }
        this.InvokeAsync("validateXML", new object[] {
                    xmlBytes,
                    validateCerts,
                    validateCertsSpecified}, this.validateXMLOperationCompleted, userState);
    }
    
    private void OnvalidateXMLOperationCompleted(object arg) {
        if ((this.validateXMLCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.validateXMLCompleted(this, new validateXMLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    public new void CancelAsync(object userState) {
        base.CancelAsync(userState);
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
public delegate void signPDFp12CompletedEventHandler(object sender, signPDFp12CompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class signPDFp12CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal signPDFp12CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public byte[] Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((byte[])(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
public delegate void signPDFCompletedEventHandler(object sender, signPDFCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class signPDFCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal signPDFCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public byte[] Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((byte[])(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
public delegate void signXMLCompletedEventHandler(object sender, signXMLCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class signXMLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal signXMLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public byte[] Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((byte[])(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
public delegate void setPDFPasswordCompletedEventHandler(object sender, setPDFPasswordCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class setPDFPasswordCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal setPDFPasswordCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public byte[] Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((byte[])(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
public delegate void signPDFServerSideCompletedEventHandler(object sender, signPDFServerSideCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class signPDFServerSideCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal signPDFServerSideCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public string Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
public delegate void signXMLRefsCompletedEventHandler(object sender, signXMLRefsCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class signXMLRefsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal signXMLRefsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public byte[] Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((byte[])(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
public delegate void validateXMLCompletedEventHandler(object sender, validateXMLCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class validateXMLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal validateXMLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public bool @return {
        get {
            this.RaiseExceptionIfNecessary();
            return ((bool)(this.results[0]));
        }
    }
    
    /// <remarks/>
    public bool returnSpecified {
        get {
            this.RaiseExceptionIfNecessary();
            return ((bool)(this.results[1]));
        }
    }
}
