using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using XafCallBack.Module.BusinessObjects;

namespace XafCallBack.Module.Web.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class SimpleCallbackExample : ViewController, IXafCallbackHandler
    {
        public SimpleCallbackExample()
        {
            InitializeComponent();
            this.TargetObjectType = typeof(DomainObject1);
            SimpleAction ExecuteScriptWithCallback = new SimpleAction(this, "ExecuteScriptWithCallback", PredefinedCategory.View);
            ExecuteScriptWithCallback.Caption = "Execute script with callback to code behind";
            ExecuteScriptWithCallback.Execute += ExecuteScriptWithCallback_Execute;
        }

        private void ExecuteScriptWithCallback_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ((WebWindow)this.Frame).RegisterStartupScript("ExecuteScriptFromAction", "RaiseXafCallback(globalCallbackControl, 'MyScript', 'Hello XAF!!! from java script', '', false);");
        }

        protected XafCallbackManager CallbackManager
        {
            get { return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager; }
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            // Perform various tasks depending on the target View.
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            //HACK you need to register the callbacks in this method "OnViewControlsCreated" otherwise the callback won't trigger
            CallbackManager.RegisterHandler("MyScript", this);
        }

        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        public void ProcessAction(string parameter)
        {
            CallbackManager.NeedEndResponse = false;
            string Script = $"alert('the script was executed and the parameter value is {parameter}')";
            ((WebWindow)this.Frame).RegisterStartupScript("CallBack", Script, true);
        }
    }
}