using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SchoolERPWebService.Data;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Filters;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Utilities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<MenuPermissionAuthorizationFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<MenuPermissionAuthorizationFilter>();
});
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Register Custom Helpers
builder.Services.AddSingleton<SqlHelper>();
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<EncryptionHelper>();

builder.Services.AddSingleton<IEmailQueueSignal, EmailQueueSignal>();
builder.Services.AddHostedService<EmailQueueWorkerService>();

builder.Services.AddSingleton<ISmsQueueSignal, SmsQueueSignal>();
builder.Services.AddHostedService<SmsQueueWorkerService>();

builder.Services.AddTransient<IMenuService, MenuService>();
builder.Services.AddTransient<IAuthServices, AuthServices>();
builder.Services.AddTransient<IUserManagementService, UserManagementService>();
builder.Services.AddTransient<ICompanyService, CompanyService>();
builder.Services.AddTransient<IUserMenuPermissionService, UserMenuPermissionService>();
builder.Services.AddTransient<SchoolERPWebService.Helpers.PermissionHelper>();
builder.Services.AddTransient<IFieldService, FieldService>();
builder.Services.AddTransient<IStudentInformationService, StudentInformationService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IRouteService, RouteService>();
builder.Services.AddTransient<IRoutePickupPointService, RoutePickupPointService>();
builder.Services.AddTransient<IHostelService, HostelService>();
builder.Services.AddTransient<IClassService, ClassService>();
builder.Services.AddTransient<ISectionService, SectionService>();
builder.Services.AddTransient<IVehicleAssignService, VehicleAssignService>();
builder.Services.AddTransient<IAttendanceService, AttendanceService>();
builder.Services.AddTransient<IAcademicsService, AcademicsService>();
builder.Services.AddTransient<IAccountEntryService, AccountEntryService>();
builder.Services.AddTransient<IAccountHeadService, AccountHeadService>();
builder.Services.AddTransient<IAlumniEventService, AlumniEventService>();
builder.Services.AddTransient<ICurrencyService, CurrencyService>();
builder.Services.AddTransient<IDownloadCenterService, DownloadCenterService>();
builder.Services.AddTransient<IEmailConfigService, EmailConfigService>();
builder.Services.AddTransient<IFrontOfficeService, FrontOfficeService>();
builder.Services.AddTransient<IHomeworkService, HomeworkService>();
builder.Services.AddTransient<IHumanResourceService, HumanResourceService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ILanguageService, LanguageService>();
builder.Services.AddTransient<ILibraryService, LibraryService>();
builder.Services.AddTransient<ILocalizationService, LocalizationService>();
builder.Services.AddTransient<IOrganisationService, OrganisationService>();
builder.Services.AddTransient<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddTransient<IPickupPointService, PickupPointService>();
builder.Services.AddTransient<ISmsConfigService, SmsConfigService>();
builder.Services.AddTransient<IStaffIDCardService, StaffIDCardService>();
builder.Services.AddTransient<IStudentCertificateService, StudentCertificateService>();
builder.Services.AddTransient<IStudentIDCardService, StudentIDCardService>();
builder.Services.AddTransient<IStudentLeaveService, StudentLeaveService>();
builder.Services.AddTransient<ISubjectGroupService, SubjectGroupService>();
builder.Services.AddTransient<ISubjectService, SubjectService>();
builder.Services.AddTransient<IVehicleService, VehicleService>();
builder.Services.AddTransient<IDashboardApiService, DashboardApiService>();
builder.Services.AddTransient<IHostelTypeService, HostelTypeService>();
builder.Services.AddTransient<IRoomCoolingTypeService, RoomCoolingTypeService>();
builder.Services.AddTransient<IHolidayTypeService, HolidayTypeService>();
builder.Services.AddTransient<IHolidayService, HolidayService>();
builder.Services.AddTransient<ICountryService, CountryService>();
builder.Services.AddTransient<IStateService, StateService>();
builder.Services.AddTransient<IWardensService, WardensService>();
builder.Services.AddTransient<ICityService, CityService>();
builder.Services.AddTransient<IPostalCodeService, PostalCodeService>();
builder.Services.AddTransient<IWeeklyHolidaysSettingService, WeeklyHolidaysSettingService>();
builder.Services.AddTransient<ILibraryBudgetService, LibraryBudgetService>();
builder.Services.AddTransient<IPublisherService, PublisherService>();
builder.Services.AddTransient<IAuthorService, AuthorService>();
builder.Services.AddTransient<ILibraryLanguageService, LibraryLanguageService>();
builder.Services.AddTransient<ILibraryCategoryService, LibraryCategoryService>();
builder.Services.AddTransient<ILibrarySubjectService, LibrarySubjectService>();
builder.Services.AddTransient<ILibraryCategorySubjectService, LibraryCategorySubjectService>();
builder.Services.AddTransient<ILibraryDocumentTypeService, LibraryDocumentTypeService>();
builder.Services.AddTransient<ILibraryDocumentStatusService, LibraryDocumentStatusService>();
builder.Services.AddTransient<ILibrarySupplierService, LibrarySupplierService>();
builder.Services.AddTransient<ILibrarySeriesService, LibrarySeriesService>();
builder.Services.AddTransient<IDailyAssignmentService, DailyAssignmentService>();
builder.Services.AddTransient<ILessonPlanService, LessonPlanService>();
builder.Services.AddTransient<ITopicService, TopicService>();
builder.Services.AddTransient<IManageLessonPlanService, ManageLessonPlanService>();
builder.Services.AddTransient<IMediaService, MediaService>();
builder.Services.AddTransient<IBackDateSettingService, BackDateSettingService>();
builder.Services.AddTransient<IItemCategoryServices, ItemCategoryServices>();
builder.Services.AddTransient<IItemStoreServices, ItemStoreServices>();
builder.Services.AddTransient<IInventoryItemService, InventoryItemService>();
builder.Services.AddTransient<IItemSupplierServicecs, ItemSupplierService>();
builder.Services.AddTransient<IItemStockService, ItemStockService>();
builder.Services.AddTransient<IIssueItemService, IssueItemService>();
builder.Services.AddTransient<IStudentInformationReportService, StudentInformationReportService>();
builder.Services.AddTransient<IAttendanceServiceReport, AttendanceServiceReport>();
builder.Services.AddTransient<ICostCenterService, CostCenterService>();
builder.Services.AddTransient<IBudgetHeadService, BudgetHeadService>();
builder.Services.AddTransient<IAccountTypeService, AccountTypeService>();
builder.Services.AddTransient<IVoucherTypeService, VoucherTypeService>();
builder.Services.AddTransient<IPaymentTypeService, PaymentTypeService>();
builder.Services.AddTransient<IPaymentTypeFieldService, PaymentTypeFieldService>();
builder.Services.AddTransient<IPaymentTypeConfigService, PaymentTypeConfigService>();
builder.Services.AddTransient<ISMSTypeService, SMSTypeService>();
builder.Services.AddTransient<ISMSTypeFieldService, SMSTypeFieldService>();
builder.Services.AddTransient<ISMSTypeConfigService, SMSTypeConfigService>();
builder.Services.AddTransient<IVoucherTypeFieldService, VoucherTypeFieldService>();
builder.Services.AddTransient<IVoucherFieldOptionService, VoucherFieldOptionService>();
builder.Services.AddTransient<IVoucherHeaderService, VoucherHeaderService>();
builder.Services.AddTransient<ISubjectTypeService, SubjectTypeService>();
builder.Services.AddTransient<IExamService, ExamService>();
builder.Services.AddTransient<IExamGroupService, ExamGroupService>();
builder.Services.AddTransient<IExamGradeService, ExamGradeService>();
builder.Services.AddTransient<IExamConfigService, ExamConfigService>();
builder.Services.AddTransient<ICoScholasticService, CoScholasticService>();
builder.Services.AddTransient<IClassCoScholasticService, ClassCoScholasticService>();
builder.Services.AddTransient<ISubSubjectService, SubSubjectService>();
builder.Services.AddTransient<IClassSubSubjectService, ClassSubSubjectService>();
builder.Services.AddTransient<IRemarkGroupService, RemarkGroupService>();
builder.Services.AddTransient<IRemarkMasterService, RemarkMasterService>();
builder.Services.AddTransient<IPaperMarksDetailsService, PaperMarksDetailsService>();
builder.Services.AddTransient<IMarksFeedingService, MarksFeedingService>();
builder.Services.AddTransient<ICoScholasticMarksFeedingService, CoScholasticMarksFeedingService>();
builder.Services.AddTransient<IAttendanceFeedingService, AttendanceFeedingService>();
builder.Services.AddTransient<IRemarksFeedingService, RemarksFeedingService>();
builder.Services.AddTransient<IPromotedClassService, PromotedClassService>();
builder.Services.AddTransient<IExamReportService, ExamReportService>();
builder.Services.AddTransient<ICBSESignOffService, CBSESignOffService>();
builder.Services.AddTransient<IStudentReExamService, StudentReExamService>();
builder.Services.AddTransient<IChangeExamService, ChangeExamService>();
builder.Services.AddTransient<IExamMarksheetDownloadPermissionServices, ExamMarksheetDownloadPermissionServices>();
builder.Services.AddTransient<ICategoryGroupService, CategoryGroupService>();
builder.Services.AddTransient<IFeeCategoryService, FeeCategoryService>();
builder.Services.AddTransient<IFeeHeadService, FeeHeadService>();
builder.Services.AddTransient<IFeeTypeService, FeeTypeService>();
builder.Services.AddTransient<IFeeTemplateService, FeeTemplateService>();
builder.Services.AddScoped<IIPaymentGatewayFactory, PaymentGatewayFactory>();
builder.Services.AddScoped<IPaymentGateway, PaymentGateway>();
builder.Services.AddScoped<IFeeCollectionService, FeeCollectionService>();
builder.Services.AddScoped<ITeacherDashboardService, TeacherDashboardService>();
builder.Services.AddScoped<IIPaymentGatewayFactory, PaymentGatewayFactory>();
builder.Services.AddScoped<IPaymentGateway, PaymentGateway>();
builder.Services.AddScoped<IFeeCollectionService, FeeCollectionService>();
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddScoped<ISMSTemplateService, SMSTemplateService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IBulkMailService, BulkMailService>();
builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();
builder.Services.AddSingleton<INoticeBoardService, NoticeBoardService>();
builder.Services.AddSingleton<ICommunicationService, CommunicationService>();

builder.Services.AddHttpClient("SmsGateway");
builder.Services.AddTransient<DynamicSmsGatewaySender>();

builder.Configuration.AddJsonFile("smsGatewayTemplates.json", optional: false, reloadOnChange: true);
builder.Services.Configure<SmsGatewayTemplateOptions>(builder.Configuration);
builder.Services.AddScoped<ISmsTemplateProvider, SmsTemplateProvider>();

// If SmsConfigService uses IDbConnection (Dapper-style) - register per your existing pattern, e.g.:
// builder.Services.AddScoped<IDbConnection>(_ => new SqlConnection(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<ISmsConfigService, SmsConfigService>();

builder.Services.AddTransient<ISmsService, SmsService>();
// Configure Global Authorization Policy
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SchoolERP_Default_Key_1234567890";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    // Default challenge returns JSON 401 for APIs
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
        }
    };
});

// Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Error handling pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolERP API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Logging middleware for unhandled exceptions
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unhandled exception: {ex}");

        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        else
        {
            throw;
        }
    }
});

app.UseHttpsRedirection();
app.UseCors("AllowMVC");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();