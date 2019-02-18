using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using XafCallBack.Module.BusinessObjects;

namespace XafCallBack.Module.DatabaseUpdate
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppUpdatingModuleUpdatertopic.aspx
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }

        private string[] GetScripts(Assembly assembly)
        {
            var executingAssembly = assembly;
            string folderName = string.Format("{0}.SampleScripts", executingAssembly.GetName().Name);
            return executingAssembly
                .GetManifestResourceNames()
                .Where(r => r.StartsWith(folderName) && r.EndsWith(".js"))
                //.Select(r => r.Substring(constantResName.Length + 1))
                .ToArray();
        }

        private Assembly assembly;

        private KeyValuePair<string, string> ReadResource(Assembly assembly, string resourceName)
        {
            var Split = resourceName.Split('.');
            var FileName = Split[Split.Length - 2];
            string Sql = string.Empty;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                Sql = reader.ReadToEnd();
            }
            var Result = new KeyValuePair<string, string>(FileName, Sql);

            return Result;
        }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            assembly = Assembly.GetExecutingAssembly();
            var Scripts = GetScripts(assembly);
            foreach (string ScriptFile in Scripts)
            {
                var Script = ReadResource(assembly, ScriptFile);
                CreateScripts(Script.Key, Script.Value);
            }

            ObjectSpace.CommitChanges(); //Uncomment this line to persist created object(s).
        }

        private void CreateScripts(string Name, string Script)
        {
            ScriptObject theObject = ObjectSpace.FindObject<ScriptObject>(CriteriaOperator.Parse("Name=?", Name));
            if (theObject == null)
            {
                theObject = ObjectSpace.CreateObject<ScriptObject>();
                theObject.Name = Name;
                theObject.Script = Script;
            }
        }

        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
    }
}