using System.Security;
using System.Security.Permissions;

//little advanced, don't worry about this
//This lets us have access to the code's private variables in our modding
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
