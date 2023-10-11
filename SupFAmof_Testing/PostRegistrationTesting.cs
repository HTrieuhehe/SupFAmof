using Xunit;
using Autofac;
using SupFAmof.Service.Service;
using SupFAmof_Testing.Attributes;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof_Testing
{
    [TestCaseOrderer("SupFAmof_Testing.PriorityOrderer", "SupFAmof_Testing")]
    public class PostRegistrationTesting : IClassFixture<DependencyInjection>
    {
        private readonly IPostRegistrationService _postRegistrationService;

        public PostRegistrationTesting(DependencyInjection dependencyInjection)
        {
            _postRegistrationService = dependencyInjection.Container
                .BeginLifetimeScope()
                .Resolve<IPostRegistrationService>();
        }

        [Fact, TestPriority(1)]
        public void GetAllPostRegistration()
        {
            int accountId = 15;
            PagingRequest request = new PagingRequest
            {
                Page = 1,
                PageSize = 3,
            };
            var result = _postRegistrationService.GetPostRegistrationByAccountId(accountId, request);
            Assert.NotNull(result.Result);
        }
        [Fact, TestPriority(2)]
        public void CreatePostRegistration_Show200Status()
        {
            int accountId = 15;
            PostRegistrationRequest request = new PostRegistrationRequest
            {
                PostId = 29,
                PositionId = 32,
                SchoolBusOption = false
            };

            var result = _postRegistrationService.CreatePostRegistration(accountId, request);
            Assert.NotNull(result.Result);
        }

        [Fact, TestPriority(3)]
        public void ApprovePostRegistrationByAdmission_Show200Status()
        {
            int accountId = 11;
            List<int> ids = new List<int>
            {
                33,38
            };
            bool approve = true;

            var result = _postRegistrationService.ApprovePostRegistrationRequest(accountId, ids,approve);
            Assert.NotNull(result.Result);
        }
    }
}
