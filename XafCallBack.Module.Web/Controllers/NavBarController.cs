using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web;
using System;
using System.Linq;

namespace XafCallBack.Module.Web.Controllers
{
    public class NavBarController : ViewController, IXafCallbackHandler
    {
        protected XafCallbackManager CallbackManager
        {
            get { return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager; }
        }
        private ASPxNavBar NavBar = null;

        public NavBarController()
        {
            //TargetWindowType = WindowType.Main;
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            //HACK you need to register the callbacks in this method "OnViewControlsCreated" otherwise the callback won't trigger
            CallbackManager.RegisterHandler("MyScript", this);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.ProcessActionContainer += Frame_ProcessActionContainer;
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            Frame.ProcessActionContainer -= Frame_ProcessActionContainer;
        }

        private void Frame_ProcessActionContainer(object sender, ProcessActionContainerEventArgs e)
        {
            if (e.ActionContainer is NavigationActionContainer)
            {
                NavBar = ((NavigationActionContainer)e.ActionContainer).NavigationControl as ASPxNavBar;
                if (NavBar != null)
                {
                    //NavBar.AllowExpanding = false;
                    NavBar.EnableClientSideAPI = true;
                    XafCallbackManager callbackManager = ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;
                    callbackManager.RegisterHandler("Javier", this);
                    var script = callbackManager.GetScript("Javier", "e.group.index");
                    //NavBar.ClientSideEvents.HeaderClick ="function(s, e){" + script + "}";
                    //NavBar.ClientSideEvents.HeaderClick = "function(s, e){RaiseXafCallback(globalCallbackControl, 'Javier', 'test', '', false);}";
                    //NavBar.ClientSideEvents.HeaderClick = @"function(s, e){" + script.Split(';')[0] + "}";
                    //NavBar.ClientSideEvents.HeaderClick = @"function(s,e){alert('OK')}";
                    NavBar.ClientSideEvents.HeaderClick = @"function(s,e){if (confirm('This action will overwrite all the current configurations. Do you want to proceed?')) {                                
                                    RaiseXafCallback(globalCallbackControl, 'script', 'true', '', false);}}";
                }
            }
        }

        public void ProcessAction(string parameter)
        {
            if (NavBar != null)
            {
                int groupIndex = Int32.Parse(parameter);
                if (groupIndex < NavBar.Groups.Count)
                {
                    NavBarGroup navBarGroup = NavBar.Groups[groupIndex];

                    if (navBarGroup.Items.Count > 0)
                        navBarGroup.Expanded = !NavBar.Groups[groupIndex].Expanded;

                    if (navBarGroup.Name == "Home")
                    {
                        IObjectSpace objectSpace = Application.CreateObjectSpace();
                        Frame.SetView(Application.CreateDashboardView(objectSpace, "HomePage", true));
                    }
                }
            }
        }
    }
}