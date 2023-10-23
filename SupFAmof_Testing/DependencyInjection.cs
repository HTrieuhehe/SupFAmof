using Autofac;
using AutoMapper;
using ServiceStack.Redis;
using SupFAmof.API.Mapper;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Data.Repository;
using SupFAmof.Service.Service;
using SupFAmof.Service.DTO.Request;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof_Testing
{
    public class DependencyInjection
    {
        private IContainer _container;

        public IContainer Container => _container;

        public DependencyInjection()
        {
            var builder = new ContainerBuilder();
            string connectionString = $"Server=13.212.21.234;" +
                                      $"Database=SupFAmOf_Stg_Db_Ver_2;" +
                                      $"User ID=sa;" +
                                      $"Password=QW0%mG0#%jRC3Z7&T4fL38ygt5Jhhx;" +
                                      $"MultipleActiveResultSets=true;" +
                                      $"Integrated Security=true;" +
                                      $"Trusted_Connection=False;" +
                                      $"Encrypt=True;" +
                                      $"TrustServerCertificate=True";
            var dbContextOptions = new DbContextOptionsBuilder<SupFAmOf_Stg_Db_Ver_2Context>()
            .UseLazyLoadingProxies()
            .UseSqlServer(connectionString)
            .Options;

            var dbContext = new SupFAmOf_Stg_Db_Ver_2Context(dbContextOptions);
            builder.RegisterInstance(dbContext).As<SupFAmOf_Stg_Db_Ver_2Context>().SingleInstance();
            var mailSettings = new MailSettings
            {
              Mail= "supfamof.dev.test@gmail.com",
                DisplayName= "SupFAmOf notification",
                Host= "smtp.gmail.com",
                Password= "mgpdrdvcwvbqjxbo",
                Port = 587,
            };
            builder.RegisterInstance<IOptions<MailSettings>>(Options.Create(mailSettings));

            var mailPaths = new MailPaths
            {
                Paths = new Dictionary<string, string>
    {
        { "VerificationMail", "/app/MailTemplate/VeryficationEmailTemplate.html" },
        { "BookingMail", "C:\\Users\\Admin\\OneDrive\\Desktop\\FPT SPRING 2023\\Đồ Án Kì 9\\SupFAmof\\SupFAmof.Serviec\\MailTemplate\\BookingEmailTemplate.html" },
        { "ContractMail", "/app/MailTemplate/Contract.html" }
    }
            };

            builder.RegisterInstance<IOptions<MailPaths>>(Options.Create(mailPaths));
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                // Add your AutoMapper profiles here
                cfg.AddProfile(new AutoMapperProfile()); // Replace AutoMapperProfile with your actual AutoMapper profile class
            })).AsSelf().SingleInstance();
            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<RoleService>().As<IRoleService>();
            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.RegisterType<AccountBankingService>().As<IAccountBankingService>();
            builder.RegisterType<ExpoTokenService>().As<IExpoTokenService>();
            builder.RegisterType<FirebaseMessagingService>().As<IFirebaseMessagingService>();
            builder.RegisterType<AdminAccountService>().As<IAdminAccountService>();
            builder.RegisterType<PostRegistrationService>().As<IPostRegistrationService>();
            builder.RegisterType<PostCategoryService>().As<IPostCategoryService>();
            builder.RegisterType<TrainingCertificateService>().As<ITrainingCertificateService>();
            builder.RegisterType<AccountCertificateService>().As<IAccountCertificateService>();
            builder.RegisterType<PostService>().As<IPostService>();
            builder.RegisterType<DocumentService>().As<IDocumentService>();
            builder.RegisterType<SendMailService>().As<ISendMailService>();
            builder.RegisterType<CheckInService>().As<ICheckInService>();
            builder.RegisterType<AccountReportService>().As<IAccountReportService>();
            builder.RegisterType<AccountBannedService>().As<IAccountBannedService>();
            builder.RegisterType<ContractService>().As<IContractService>();
            builder.RegisterGeneric(typeof(GenericRepository<>))
            .As(typeof(IGenericRepository<>))
            .InstancePerLifetimeScope();

            _container = builder.Build();
        }
    }
}
