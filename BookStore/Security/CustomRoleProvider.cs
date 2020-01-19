using BookHive.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BookHive.Security
{
    
    public class CustomRoleProvider:RoleProvider 
    {
        private string ConnectionString = "Data Source=.; Initial Catalog=BookHive;Integrated Security=SSPI";
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles() //implemented
        {
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("Select distinct UserRole from UserAccount", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            List<string> Roles = new List<string>();
            while (dr.Read())
            {
                Roles.Add(Convert.ToString(dr["UserRole"]));
                //Roles[1] = Convert.ToString(dr["UserRole"]);
            }
            return Roles.ToArray();
        }

        public override string[] GetRolesForUser(string username) // to implement
        {
            
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("Select UserRole from UserAccount where UserName='"+username+"'", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            List<string> Roles = new List<string>();
            while (dr.Read())
            {
                if (Convert.ToString(dr["UserRole"]) != null)
                {
                    Roles.Add(Convert.ToString(dr["UserRole"]));
                }
               
            }
            return Roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)//to implement
        {
            
            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand("Select UserRole from UserAccount where UserName='" + username+"'", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (username == Convert.ToString(dr["UserName"]) && roleName == Convert.ToString(dr["UserRole"]))
                {
                    return true;
                }
            }
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}