using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Description;
using System.Web;

namespace ServiceCRM.Models
{
    public class CrmHelper : IDisposable
    {
        public void Dispose()
        {

        }
        string callMethod;
        public string StartsWork(string inputNumber)
        {
            try
            {
                callMethod = new StackTrace(false).GetFrame(0).GetMethod().Name;
                IOrganizationService service = ConnectToCRM();
                EntityCollection entites = GetEntities(service);
                string result = CheckFields(entites, inputNumber, service);
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string StopsWork(string shortNumber)
        {
            try
            {
                callMethod = new StackTrace(false).GetFrame(0).GetMethod().Name;
                IOrganizationService service = ConnectToCRM();
                EntityCollection entites = GetEntities(service);
                string result = CheckFields(entites, shortNumber, service);
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        private EntityCollection GetEntities(IOrganizationService service)
        {
            EntityCollection collection = null;
            QueryExpression query = new QueryExpression("contact");
            query.ColumnSet.AddColumns("fullname", "new_authenticated", "new_shortnumber");
            collection = service.RetrieveMultiple(query);
            return collection;
        }
        private IOrganizationService ConnectToCRM()
        {
            IOrganizationService service = null;
            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = "username";
            clientCredentials.UserName.Password = "password";
            service = (IOrganizationService)new OrganizationServiceProxy(new Uri("http://XX.XX.XX.XXX/LearnAPetukhov/XRMServices/2011/Organization.svc"),
             null, clientCredentials, null);
            return service;
        }
        private string CheckFields(EntityCollection entites, string inputNumber, IOrganizationService service)
        {
            string result = "all right";

            foreach (Entity itemEntity in entites.Entities)
            {
                try
                {
                    Guid entityId = new Guid(itemEntity.Attributes["contactid"].ToString());
                    var auth = itemEntity.Attributes["new_authenticated"].ToString();
                    string num = itemEntity.Attributes["new_shortnumber"].ToString();
                    if (num.Equals(inputNumber))
                    {
                        if (auth.Equals("False") && callMethod.Equals("StartsWork"))
                        {
                            itemEntity.Attributes["new_authenticated"] = true;
                        } 
                        else if (auth.Equals("True") && callMethod.Equals("StopsWork"))
                        {
                            itemEntity.Attributes["new_authenticated"] = false;
                        }
                        service.Update(itemEntity);
                        return result;
                    }
                }
                catch { }
            }
            return "no matches for the entered number";
        }
        //private void ChangeFieldValue(IOrganizationService service, Entity itemEntity)
        //{
        //    itemEntity.Attributes["new_authenticated"] = true;
        //    service.Update(itemEntity);
        //}
    }
}
