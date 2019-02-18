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
    public partial class CallbackController : ViewController, IXafCallbackHandler
    {
        public CallbackController()
        {
            InitializeComponent();
            this.TargetObjectType = typeof(ScriptObject);
            SimpleAction ExecuteCurrentScript = new SimpleAction(this, "Execute current script", PredefinedCategory.View);
            ExecuteCurrentScript.Caption = "Execute current script";
            ExecuteCurrentScript.Execute += ExecuteScript;
            ExecuteCurrentScript.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }

        private void ExecuteScript(object sender, SimpleActionExecuteEventArgs e)
        {
            ScriptObject CurrentObject = (ScriptObject)e.CurrentObject;
            ((WebWindow)this.Frame).RegisterStartupScript(CurrentObject.Name, CurrentObject.Script, true);
        }

        private void RegisterXafCallackHandler()
        {
            if (XafCallbackManager != null)
            {
                var AllScripts = this.ObjectSpace.GetObjects<ScriptObject>();
                foreach (var item in AllScripts)
                {
                    XafCallbackManager.RegisterHandler(item.Name, this);
                }
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            RegisterXafCallackHandler();
        }

        protected XafCallbackManager XafCallbackManager
        {
            get
            {
                return WebWindow.CurrentRequestPage != null ? ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager : null;
            }
        }

        public void ProcessAction(string parameter)
        {
            XafCallbackManager.NeedEndResponse = false;
            string Script = $"alert(Parameter value;'{parameter}');";
            ((WebWindow)this.Frame).RegisterStartupScript("CallBackAlert", Script, true);
        }
    }
}