﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAES.Infrastructure.ServiceReferenceHSM {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EncabezadoRequest", Namespace="http://www.e-sign.cl/")]
    [System.SerializableAttribute()]
    public partial class EncabezadoRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UserField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PasswordField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TipoIntercambioField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NombreConfiguracionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FormatoDocumentoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RespuestaEsperadaField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string User {
            get {
                return this.UserField;
            }
            set {
                if ((object.ReferenceEquals(this.UserField, value) != true)) {
                    this.UserField = value;
                    this.RaisePropertyChanged("User");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string Password {
            get {
                return this.PasswordField;
            }
            set {
                if ((object.ReferenceEquals(this.PasswordField, value) != true)) {
                    this.PasswordField = value;
                    this.RaisePropertyChanged("Password");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string TipoIntercambio {
            get {
                return this.TipoIntercambioField;
            }
            set {
                if ((object.ReferenceEquals(this.TipoIntercambioField, value) != true)) {
                    this.TipoIntercambioField = value;
                    this.RaisePropertyChanged("TipoIntercambio");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string NombreConfiguracion {
            get {
                return this.NombreConfiguracionField;
            }
            set {
                if ((object.ReferenceEquals(this.NombreConfiguracionField, value) != true)) {
                    this.NombreConfiguracionField = value;
                    this.RaisePropertyChanged("NombreConfiguracion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string FormatoDocumento {
            get {
                return this.FormatoDocumentoField;
            }
            set {
                if ((object.ReferenceEquals(this.FormatoDocumentoField, value) != true)) {
                    this.FormatoDocumentoField = value;
                    this.RaisePropertyChanged("FormatoDocumento");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string RespuestaEsperada {
            get {
                return this.RespuestaEsperadaField;
            }
            set {
                if ((object.ReferenceEquals(this.RespuestaEsperadaField, value) != true)) {
                    this.RespuestaEsperadaField = value;
                    this.RaisePropertyChanged("RespuestaEsperada");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DocumentoParametro", Namespace="http://www.e-sign.cl/")]
    [System.SerializableAttribute()]
    public partial class DocumentoParametro : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DocumentoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NombreDocumentoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MetaDataField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string Documento {
            get {
                return this.DocumentoField;
            }
            set {
                if ((object.ReferenceEquals(this.DocumentoField, value) != true)) {
                    this.DocumentoField = value;
                    this.RaisePropertyChanged("Documento");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string NombreDocumento {
            get {
                return this.NombreDocumentoField;
            }
            set {
                if ((object.ReferenceEquals(this.NombreDocumentoField, value) != true)) {
                    this.NombreDocumentoField = value;
                    this.RaisePropertyChanged("NombreDocumento");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string MetaData {
            get {
                return this.MetaDataField;
            }
            set {
                if ((object.ReferenceEquals(this.MetaDataField, value) != true)) {
                    this.MetaDataField = value;
                    this.RaisePropertyChanged("MetaData");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EncabezadoResponse", Namespace="http://www.e-sign.cl/")]
    [System.SerializableAttribute()]
    public partial class EncabezadoResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EstadoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ComentariosField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FormatoDocumentoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NombreDocumentoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DocumentoField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string Estado {
            get {
                return this.EstadoField;
            }
            set {
                if ((object.ReferenceEquals(this.EstadoField, value) != true)) {
                    this.EstadoField = value;
                    this.RaisePropertyChanged("Estado");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string Comentarios {
            get {
                return this.ComentariosField;
            }
            set {
                if ((object.ReferenceEquals(this.ComentariosField, value) != true)) {
                    this.ComentariosField = value;
                    this.RaisePropertyChanged("Comentarios");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string FormatoDocumento {
            get {
                return this.FormatoDocumentoField;
            }
            set {
                if ((object.ReferenceEquals(this.FormatoDocumentoField, value) != true)) {
                    this.FormatoDocumentoField = value;
                    this.RaisePropertyChanged("FormatoDocumento");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string NombreDocumento {
            get {
                return this.NombreDocumentoField;
            }
            set {
                if ((object.ReferenceEquals(this.NombreDocumentoField, value) != true)) {
                    this.NombreDocumentoField = value;
                    this.RaisePropertyChanged("NombreDocumento");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string Documento {
            get {
                return this.DocumentoField;
            }
            set {
                if ((object.ReferenceEquals(this.DocumentoField, value) != true)) {
                    this.DocumentoField = value;
                    this.RaisePropertyChanged("Documento");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.e-sign.cl/", ConfigurationName="ServiceReferenceHSM.WSIntercambiaDocSoap")]
    public interface WSIntercambiaDocSoap {
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento Encabezado del espacio de nombres http://www.e-sign.cl/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.e-sign.cl/IntercambiaDoc", ReplyAction="*")]
        DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocResponse IntercambiaDoc(DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.e-sign.cl/IntercambiaDoc", ReplyAction="*")]
        System.Threading.Tasks.Task<DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocResponse> IntercambiaDocAsync(DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequest request);
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento MantencionDeSistemaResult del espacio de nombres http://www.e-sign.cl/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.e-sign.cl/MantencionDeSistema", ReplyAction="*")]
        DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaResponse MantencionDeSistema(DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.e-sign.cl/MantencionDeSistema", ReplyAction="*")]
        System.Threading.Tasks.Task<DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaResponse> MantencionDeSistemaAsync(DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class IntercambiaDocRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="IntercambiaDoc", Namespace="http://www.e-sign.cl/", Order=0)]
        public DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequestBody Body;
        
        public IntercambiaDocRequest() {
        }
        
        public IntercambiaDocRequest(DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.e-sign.cl/")]
    public partial class IntercambiaDocRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public DAES.Infrastructure.ServiceReferenceHSM.EncabezadoRequest Encabezado;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public DAES.Infrastructure.ServiceReferenceHSM.DocumentoParametro Parametro;
        
        public IntercambiaDocRequestBody() {
        }
        
        public IntercambiaDocRequestBody(DAES.Infrastructure.ServiceReferenceHSM.EncabezadoRequest Encabezado, DAES.Infrastructure.ServiceReferenceHSM.DocumentoParametro Parametro) {
            this.Encabezado = Encabezado;
            this.Parametro = Parametro;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class IntercambiaDocResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="IntercambiaDocResponse", Namespace="http://www.e-sign.cl/", Order=0)]
        public DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocResponseBody Body;
        
        public IntercambiaDocResponse() {
        }
        
        public IntercambiaDocResponse(DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.e-sign.cl/")]
    public partial class IntercambiaDocResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public DAES.Infrastructure.ServiceReferenceHSM.EncabezadoResponse IntercambiaDocResult;
        
        public IntercambiaDocResponseBody() {
        }
        
        public IntercambiaDocResponseBody(DAES.Infrastructure.ServiceReferenceHSM.EncabezadoResponse IntercambiaDocResult) {
            this.IntercambiaDocResult = IntercambiaDocResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class MantencionDeSistemaRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="MantencionDeSistema", Namespace="http://www.e-sign.cl/", Order=0)]
        public DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequestBody Body;
        
        public MantencionDeSistemaRequest() {
        }
        
        public MantencionDeSistemaRequest(DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class MantencionDeSistemaRequestBody {
        
        public MantencionDeSistemaRequestBody() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class MantencionDeSistemaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="MantencionDeSistemaResponse", Namespace="http://www.e-sign.cl/", Order=0)]
        public DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaResponseBody Body;
        
        public MantencionDeSistemaResponse() {
        }
        
        public MantencionDeSistemaResponse(DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.e-sign.cl/")]
    public partial class MantencionDeSistemaResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string MantencionDeSistemaResult;
        
        public MantencionDeSistemaResponseBody() {
        }
        
        public MantencionDeSistemaResponseBody(string MantencionDeSistemaResult) {
            this.MantencionDeSistemaResult = MantencionDeSistemaResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WSIntercambiaDocSoapChannel : DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WSIntercambiaDocSoapClient : System.ServiceModel.ClientBase<DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap>, DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap {
        
        public WSIntercambiaDocSoapClient() {
        }
        
        public WSIntercambiaDocSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WSIntercambiaDocSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WSIntercambiaDocSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WSIntercambiaDocSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocResponse DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap.IntercambiaDoc(DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequest request) {
            return base.Channel.IntercambiaDoc(request);
        }
        
        public DAES.Infrastructure.ServiceReferenceHSM.EncabezadoResponse IntercambiaDoc(DAES.Infrastructure.ServiceReferenceHSM.EncabezadoRequest Encabezado, DAES.Infrastructure.ServiceReferenceHSM.DocumentoParametro Parametro) {
            DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequest inValue = new DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequest();
            inValue.Body = new DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequestBody();
            inValue.Body.Encabezado = Encabezado;
            inValue.Body.Parametro = Parametro;
            DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocResponse retVal = ((DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap)(this)).IntercambiaDoc(inValue);
            return retVal.Body.IntercambiaDocResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocResponse> DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap.IntercambiaDocAsync(DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequest request) {
            return base.Channel.IntercambiaDocAsync(request);
        }
        
        public System.Threading.Tasks.Task<DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocResponse> IntercambiaDocAsync(DAES.Infrastructure.ServiceReferenceHSM.EncabezadoRequest Encabezado, DAES.Infrastructure.ServiceReferenceHSM.DocumentoParametro Parametro) {
            DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequest inValue = new DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequest();
            inValue.Body = new DAES.Infrastructure.ServiceReferenceHSM.IntercambiaDocRequestBody();
            inValue.Body.Encabezado = Encabezado;
            inValue.Body.Parametro = Parametro;
            return ((DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap)(this)).IntercambiaDocAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaResponse DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap.MantencionDeSistema(DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequest request) {
            return base.Channel.MantencionDeSistema(request);
        }
        
        public string MantencionDeSistema() {
            DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequest inValue = new DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequest();
            inValue.Body = new DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequestBody();
            DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaResponse retVal = ((DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap)(this)).MantencionDeSistema(inValue);
            return retVal.Body.MantencionDeSistemaResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaResponse> DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap.MantencionDeSistemaAsync(DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequest request) {
            return base.Channel.MantencionDeSistemaAsync(request);
        }
        
        public System.Threading.Tasks.Task<DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaResponse> MantencionDeSistemaAsync() {
            DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequest inValue = new DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequest();
            inValue.Body = new DAES.Infrastructure.ServiceReferenceHSM.MantencionDeSistemaRequestBody();
            return ((DAES.Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoap)(this)).MantencionDeSistemaAsync(inValue);
        }
    }
}
