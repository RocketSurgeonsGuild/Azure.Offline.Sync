using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Rocket.Surgery.Azure.Sync.Abstractions;

namespace Rocket.Surgery.Azure.Sync
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMobileServiceClient _mobileServiceClient;

        public AuthenticationService(IMobileServiceClient mobileServiceClient)
        {
            _mobileServiceClient = mobileServiceClient;
        }

        public async Task<IAuthenticationResponse> Authenticate(IAuthenticationRequest request)
        {
            MobileServiceUser user = await _mobileServiceClient.LoginAsync("", JObject.FromObject(request));
            return default(IAuthenticationResponse);
        }

        public T DeserializeToken<T>(string authToken) where T : IJwt
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }

    public class MockAuthenticationService : IAuthenticationService
    {
        private readonly ISession _session;

        public MockAuthenticationService(ISession session)
        {
            _session = session;
        }

        public async Task<IAuthenticationResponse> Authenticate(IAuthenticationRequest request)
        {
//            Authentication.UserName = UserName;
//
//            AuthenticationResponse retVal = new AuthenticationResponse();
//
//            retVal.IsAuthenticated = (Password == "Monday12!");
//            retVal.PersonID = Guid.NewGuid().ToString();
//            retVal.Email = UserName;
//
//            switch (UserName.ToLower())
//            {
//                case "error@school.com":
//                    retVal.FirstName = "Jon";
//                    retVal.LastName = "Doe";
//                    retVal.Role = RoleType.IncidentCommander;
//                    break;
//                case "george@school.com":
//                    retVal.FirstName = "George";
//                    retVal.LastName = "Costanza";
//                    retVal.Role = RoleType.IncidentCommander;
//                    AppSettings.ActiveIncidentInProgress = false;
//                    AppSettings.ActiveIncidentID = null;
//                    retVal.PersonID = "A7BB5138-B95F-4B7A-BF64-0000758F2694";
//                    retVal.CanInitiateAndEndIncident = true;
//                    retVal.IsIncidentCommander = true;
//                    break;
//                case "mary@school.com":
//                    retVal.FirstName = "Mary";
//                    retVal.LastName = "Supercalifragilisticexpialidocious";
//                    retVal.Role = RoleType.StudentSupervisor;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = Guid.NewGuid().ToString();
//                    retVal.PersonID = "F47E9478-76CC-4B5E-B575-00010C81B116";
//
//                    break;
//                case "carrie@school.com":
//                    retVal.FirstName = "Carrie";
//                    retVal.LastName = "Bradshaw";
//                    retVal.Role = RoleType.StudentSupervisor;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = Guid.NewGuid().ToString();
//                    retVal.PersonID = "5A547867-35AB-4F53-8895-00036CBEDFA6";
//                    break;
//                case "suzie@school.com":
//                    retVal.FirstName = "Suzanne";
//                    retVal.LastName = "Warren";
//                    retVal.Role = RoleType.StudentSupervisor;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = Guid.NewGuid().ToString();
//                    retVal.PersonID = "23166F75-1E86-4B02-9648-00040E2DBA68";
//                    break;
//                case "marge@school.com":
//                    retVal.FirstName = "Marge";
//                    retVal.LastName = "Simpson";
//                    retVal.Role = RoleType.StudentSupervisor;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = Guid.NewGuid().ToString();
//                    retVal.PersonID = "DB191FFB-55C0-411F-917F-0004DF966049";
//                    break;
//                case "wilson@school.com":
//                    retVal.FirstName = "Wilson";
//                    retVal.LastName = "Wilson";
//                    retVal.Role = RoleType.StudentSupervisor;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = Guid.NewGuid().ToString();
//                    retVal.PersonID = "02E5C6E5-718F-405A-BF5F-0005F452906A";
//                    retVal.RequiresPasswordUpdate = true;
//                    break;
//                case "dana@school.com":
//                    retVal.FirstName = "Dana";
//                    retVal.LastName = "Scully";
//                    retVal.Role = RoleType.StudentSupervisor;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = Guid.NewGuid().ToString();
//                    retVal.PersonID = "DC2E3FD5-BBAD-4F1B-9690-001064DBBD0D";
//                    retVal.RequiresPasswordUpdate = true;
//                    break;
//                case "rick@school.com":
//                    retVal.FirstName = "Rick";
//                    retVal.LastName = "Grimes";
//                    retVal.Role = RoleType.StudentSupervisor;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = Guid.NewGuid().ToString();
//                    retVal.PersonID = "5FC4D18E-5E49-4326-84B5-0010E6B729AC";
//                    break;
//                case "khaleesi@school.com":
//                    retVal.FirstName = "Daenerys";
//                    retVal.LastName = "Targaryen";
//                    retVal.Role = RoleType.StudentSupervisor;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = Guid.NewGuid().ToString();
//                    retVal.PersonID = "CFB94448-D060-4F4E-93B8-00116CB91557";
//                    break;
//                case "rosewood@school.com":
//                    retVal.FirstName = "Beaumont";
//                    retVal.LastName = "Rosewood";
//                    retVal.Role = RoleType.IncidentCommander;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = "c7045b94-d92d-4ae0-8d4c-390effb35753";
//                    retVal.PersonID = "DB5383F6-9D81-4CF4-8734-0013C872D45E";
//                    retVal.CanInitiateAndEndIncident = true;
//                    retVal.IsIncidentCommander = true;
//                    break;
//                case "bellum@school.com":
//                    retVal.FirstName = "Sara";
//                    retVal.LastName = "Bellum";
//                    retVal.Role = RoleType.IncidentCommander;
//                    AppSettings.ActiveIncidentInProgress = true;
//                    AppSettings.ActiveIncidentID = "c7045b94-d92d-4ae0-8d4c-390effb35753";
//                    retVal.PersonID = "FE511FF4-3518-49D6-9C94-1A5C8A6AC384";
//                    retVal.CanInitiateAndEndIncident = true;
//                    retVal.IsIncidentCommander = true;
//                    break;
//                default:
//                    retVal.IsAuthenticated = false;
//                    break;
//            }
//
//            if (retVal.Role == RoleType.IncidentCommander)
//            {
//                retVal.CanInitiateAndEndIncident = true;
//                retVal.IsIncidentCommander = true;
//                retVal.CanPerformReunificationTasks = true;
//            }
//
//            if (retVal.Role == RoleType.StudentSupervisor)
//            {
//                retVal.CanInitiateAndEndIncident = false;
//                retVal.IsStudentSupervisor = true;
//                retVal.CanPerformReunificationTasks = true;
//
//            }
//            await Task.Delay(500);
            return await Task.FromResult(default(IAuthenticationResponse));
        }

        public T DeserializeToken<T>(string authToken) where T : IJwt
        {
            return default(T);
        }

        public void Logout()
        {
        }
    }
}
