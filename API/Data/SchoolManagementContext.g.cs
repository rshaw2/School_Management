using Microsoft.EntityFrameworkCore;
using SchoolManagement.Entities;

namespace SchoolManagement.Data
{
    /// <summary>
    /// Represents the database context for the application.
    /// </summary>
    public class SchoolManagementContext : DbContext
    {
        /// <summary>
        /// Configures the database connection options.
        /// </summary>
        /// <param name="optionsBuilder">The options builder used to configure the database connection.</param>
        protected override void OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=june122024.database.windows.net;Initial Catalog=T423798_School_Management;Persist Security Info=True;user id=Lowcodeadmin;password=NtLowCode^123*;Integrated Security=false;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=true;");
        }

        /// <summary>
        /// Configures the model relationships and entity mappings.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the database model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInRole>().HasKey(a => a.Id);
            modelBuilder.Entity<UserToken>().HasKey(a => a.Id);
            modelBuilder.Entity<RoleEntitlement>().HasKey(a => a.Id);
            modelBuilder.Entity<Entity>().HasKey(a => a.Id);
            modelBuilder.Entity<Tenant>().HasKey(a => a.Id);
            modelBuilder.Entity<User>().HasKey(a => a.Id);
            modelBuilder.Entity<Role>().HasKey(a => a.Id);
            modelBuilder.Entity<Student>().HasKey(a => a.Id);
            modelBuilder.Entity<ParentGuardian>().HasKey(a => a.Id);
            modelBuilder.Entity<ContactInformation>().HasKey(a => a.Id);
            modelBuilder.Entity<Address>().HasKey(a => a.Id);
            modelBuilder.Entity<Course>().HasKey(a => a.Id);
            modelBuilder.Entity<Enrollment>().HasKey(a => a.Id);
            modelBuilder.Entity<Attendance>().HasKey(a => a.Id);
            modelBuilder.Entity<Grade>().HasKey(a => a.Id);
            modelBuilder.Entity<Transcript>().HasKey(a => a.Id);
            modelBuilder.Entity<Assignment>().HasKey(a => a.Id);
            modelBuilder.Entity<Exam>().HasKey(a => a.Id);
            modelBuilder.Entity<ExtraCurricularActivity>().HasKey(a => a.Id);
            modelBuilder.Entity<Schedule>().HasKey(a => a.Id);
            modelBuilder.Entity<Fee>().HasKey(a => a.Id);
            modelBuilder.Entity<Payment>().HasKey(a => a.Id);
            modelBuilder.Entity<Discount>().HasKey(a => a.Id);
            modelBuilder.Entity<Billing>().HasKey(a => a.Id);
            modelBuilder.Entity<ReportCard>().HasKey(a => a.Id);
            modelBuilder.Entity<AcademicYear>().HasKey(a => a.Id);
            modelBuilder.Entity<Semester>().HasKey(a => a.Id);
            modelBuilder.Entity<Class>().HasKey(a => a.Id);
            modelBuilder.Entity<Section>().HasKey(a => a.Id);
            modelBuilder.Entity<Classroom>().HasKey(a => a.Id);
            modelBuilder.Entity<Faculty>().HasKey(a => a.Id);
            modelBuilder.Entity<Staff>().HasKey(a => a.Id);
            modelBuilder.Entity<NoticeBoard>().HasKey(a => a.Id);
            modelBuilder.Entity<SchoolEvents>().HasKey(a => a.Id);
            modelBuilder.Entity<Library>().HasKey(a => a.Id);
            modelBuilder.Entity<DocumentTypes>().HasKey(a => a.Id);
            modelBuilder.Entity<StudentDocuments>().HasKey(a => a.Id);
            modelBuilder.Entity<Employee>().HasKey(a => a.Id);
            modelBuilder.Entity<Department>().HasKey(a => a.Id);
            modelBuilder.Entity<JobTitle>().HasKey(a => a.Id);
            modelBuilder.Entity<EmploymentStatus>().HasKey(a => a.Id);
            modelBuilder.Entity<Shift>().HasKey(a => a.Id);
            modelBuilder.Entity<TimeOffRequest>().HasKey(a => a.Id);
            modelBuilder.Entity<Leave>().HasKey(a => a.Id);
            modelBuilder.Entity<PerformanceReview>().HasKey(a => a.Id);
            modelBuilder.Entity<Payroll>().HasKey(a => a.Id);
            modelBuilder.Entity<Salary>().HasKey(a => a.Id);
            modelBuilder.Entity<Benefits>().HasKey(a => a.Id);
            modelBuilder.Entity<Skill>().HasKey(a => a.Id);
            modelBuilder.Entity<Training>().HasKey(a => a.Id);
            modelBuilder.Entity<Document>().HasKey(a => a.Id);
            modelBuilder.Entity<EmergencyContact>().HasKey(a => a.Id);
            modelBuilder.Entity<WorkAnnouncement>().HasKey(a => a.Id);
            modelBuilder.Entity<Teacher>().HasKey(a => a.Id);
            modelBuilder.Entity<Students>().HasKey(a => a.Id);
            modelBuilder.Entity<CourseSchedule>().HasKey(a => a.Id);
            modelBuilder.Entity<Instructor>().HasKey(a => a.Id);
            modelBuilder.Entity<Room>().HasKey(a => a.Id);
            modelBuilder.Entity<Building>().HasKey(a => a.Id);
            modelBuilder.Entity<Campus>().HasKey(a => a.Id);
            modelBuilder.Entity<AcademicSession>().HasKey(a => a.Id);
            modelBuilder.Entity<ClassPeriod>().HasKey(a => a.Id);
            modelBuilder.Entity<TimetableTemplate>().HasKey(a => a.Id);
            modelBuilder.Entity<Holidays>().HasKey(a => a.Id);
            modelBuilder.Entity<Breaks>().HasKey(a => a.Id);
            modelBuilder.Entity<ClassType>().HasKey(a => a.Id);
            modelBuilder.Entity<ConflictResolution>().HasKey(a => a.Id);
            modelBuilder.Entity<MeetingPattern>().HasKey(a => a.Id);
            modelBuilder.Entity<ResourceAllocation>().HasKey(a => a.Id);
            modelBuilder.Entity<TimeSlot>().HasKey(a => a.Id);
            modelBuilder.Entity<TimeConstraints>().HasKey(a => a.Id);
            modelBuilder.Entity<Resource>().HasKey(a => a.Id);
            modelBuilder.Entity<TimeLog>().HasKey(a => a.Id);
            modelBuilder.Entity<WorkSchedule>().HasKey(a => a.Id);
            modelBuilder.Entity<Absence>().HasKey(a => a.Id);
            modelBuilder.Entity<AbsenceType>().HasKey(a => a.Id);
            modelBuilder.Entity<Overtime>().HasKey(a => a.Id);
            modelBuilder.Entity<TimeOff>().HasKey(a => a.Id);
            modelBuilder.Entity<TimeOffApproval>().HasKey(a => a.Id);
            modelBuilder.Entity<LeaveBalance>().HasKey(a => a.Id);
            modelBuilder.Entity<PayPeriod>().HasKey(a => a.Id);
            modelBuilder.Entity<AttendanceReport>().HasKey(a => a.Id);
            modelBuilder.Entity<AttendancePolicy>().HasKey(a => a.Id);
            modelBuilder.Entity<LeaveType>().HasKey(a => a.Id);
            modelBuilder.Entity<FeeCategory>().HasKey(a => a.Id);
            modelBuilder.Entity<FeeItem>().HasKey(a => a.Id);
            modelBuilder.Entity<FeeSchedule>().HasKey(a => a.Id);
            modelBuilder.Entity<Invoice>().HasKey(a => a.Id);
            modelBuilder.Entity<InvoiceLineItem>().HasKey(a => a.Id);
            modelBuilder.Entity<PaymentMethod>().HasKey(a => a.Id);
            modelBuilder.Entity<Currency>().HasKey(a => a.Id);
            modelBuilder.Entity<Tax>().HasKey(a => a.Id);
            modelBuilder.Entity<BillingFrequency>().HasKey(a => a.Id);
            modelBuilder.Entity<FinancialAccount>().HasKey(a => a.Id);
            modelBuilder.Entity<AccountTransaction>().HasKey(a => a.Id);
            modelBuilder.Entity<AccountReconciliation>().HasKey(a => a.Id);
            modelBuilder.Entity<PaymentGateway>().HasKey(a => a.Id);
            modelBuilder.Entity<PaymentStatus>().HasKey(a => a.Id);
            modelBuilder.Entity<LateFee>().HasKey(a => a.Id);
            modelBuilder.Entity<Refund>().HasKey(a => a.Id);
            modelBuilder.Entity<PaymentTerms>().HasKey(a => a.Id);
            modelBuilder.Entity<BillingCycle>().HasKey(a => a.Id);
            modelBuilder.Entity<FeeAdjustment>().HasKey(a => a.Id);
            modelBuilder.Entity<School>().HasKey(a => a.Id);
            modelBuilder.Entity<Term>().HasKey(a => a.Id);
            modelBuilder.Entity<Installment>().HasKey(a => a.Id);
            modelBuilder.Entity<FeeWaiver>().HasKey(a => a.Id);
            modelBuilder.Entity<ExamSchedule>().HasKey(a => a.Id);
            modelBuilder.Entity<Result>().HasKey(a => a.Id);
            modelBuilder.Entity<ResultDetails>().HasKey(a => a.Id);
            modelBuilder.Entity<ExamSubject>().HasKey(a => a.Id);
            modelBuilder.Entity<Question>().HasKey(a => a.Id);
            modelBuilder.Entity<Answer>().HasKey(a => a.Id);
            modelBuilder.Entity<QuestionType>().HasKey(a => a.Id);
            modelBuilder.Entity<QuestionCategory>().HasKey(a => a.Id);
            modelBuilder.Entity<ExamRoom>().HasKey(a => a.Id);
            modelBuilder.Entity<GradingScale>().HasKey(a => a.Id);
            modelBuilder.Entity<Examiner>().HasKey(a => a.Id);
            modelBuilder.Entity<ExaminationBoard>().HasKey(a => a.Id);
            modelBuilder.Entity<ExamSession>().HasKey(a => a.Id);
            modelBuilder.Entity<ExamPaper>().HasKey(a => a.Id);
            modelBuilder.Entity<MarksDistribution>().HasKey(a => a.Id);
            modelBuilder.Entity<AttendanceStatus>().HasKey(a => a.Id);
            modelBuilder.Entity<Certificate>().HasKey(a => a.Id);
            modelBuilder.Entity<ExamResultNotification>().HasKey(a => a.Id);
            modelBuilder.Entity<EvaluationCriteria>().HasKey(a => a.Id);
            modelBuilder.Entity<ExamResultTemplate>().HasKey(a => a.Id);
            modelBuilder.Entity<Email>().HasKey(a => a.Id);
            modelBuilder.Entity<Chat>().HasKey(a => a.Id);
            modelBuilder.Entity<Calendar>().HasKey(a => a.Id);
            modelBuilder.Entity<Meeting>().HasKey(a => a.Id);
            modelBuilder.Entity<Event>().HasKey(a => a.Id);
            modelBuilder.Entity<Contact>().HasKey(a => a.Id);
            modelBuilder.Entity<Folder>().HasKey(a => a.Id);
            modelBuilder.Entity<Message>().HasKey(a => a.Id);
            modelBuilder.Entity<Notification>().HasKey(a => a.Id);
            modelBuilder.Entity<Project>().HasKey(a => a.Id);
            modelBuilder.Entity<Workspace>().HasKey(a => a.Id);
            modelBuilder.Entity<Content>().HasKey(a => a.Id);
            modelBuilder.Entity<KnowledgeBase>().HasKey(a => a.Id);
            modelBuilder.Entity<Comment>().HasKey(a => a.Id);
            modelBuilder.Entity<Poll>().HasKey(a => a.Id);
            modelBuilder.Entity<Survey>().HasKey(a => a.Id);
            modelBuilder.Entity<Attachment>().HasKey(a => a.Id);
            modelBuilder.Entity<Channel>().HasKey(a => a.Id);
            modelBuilder.Entity<Discussion>().HasKey(a => a.Id);
            modelBuilder.Entity<VideoConference>().HasKey(a => a.Id);
            modelBuilder.Entity<Agenda>().HasKey(a => a.Id);
            modelBuilder.Entity<Minutes>().HasKey(a => a.Id);
            modelBuilder.Entity<StatusUpdate>().HasKey(a => a.Id);
            modelBuilder.Entity<ActivityFeed>().HasKey(a => a.Id);
            modelBuilder.Entity<Tag>().HasKey(a => a.Id);
            modelBuilder.Entity<Category>().HasKey(a => a.Id);
            modelBuilder.Entity<Versioning>().HasKey(a => a.Id);
            modelBuilder.Entity<DocumentType>().HasKey(a => a.Id);
            modelBuilder.Entity<DocumentCategory>().HasKey(a => a.Id);
            modelBuilder.Entity<DocumentStatus>().HasKey(a => a.Id);
            modelBuilder.Entity<DocumentVersion>().HasKey(a => a.Id);
            modelBuilder.Entity<ResourceType>().HasKey(a => a.Id);
            modelBuilder.Entity<ResourceCategory>().HasKey(a => a.Id);
            modelBuilder.Entity<ResourceStatus>().HasKey(a => a.Id);
            modelBuilder.Entity<AccessLevel>().HasKey(a => a.Id);
            modelBuilder.Entity<Tags>().HasKey(a => a.Id);
            modelBuilder.Entity<FileStorage>().HasKey(a => a.Id);
            modelBuilder.Entity<Share>().HasKey(a => a.Id);
            modelBuilder.Entity<History>().HasKey(a => a.Id);
            modelBuilder.Entity<Permissions>().HasKey(a => a.Id);
            modelBuilder.Entity<Workflow>().HasKey(a => a.Id);
            modelBuilder.Entity<Approval>().HasKey(a => a.Id);
            modelBuilder.Entity<WorkflowStep>().HasKey(a => a.Id);
            modelBuilder.Entity<StorageProvider>().HasKey(a => a.Id);
            modelBuilder.Entity<SearchCriteria>().HasKey(a => a.Id);
            modelBuilder.Entity<DocumentTemplate>().HasKey(a => a.Id);
            modelBuilder.Entity<ResourceRequest>().HasKey(a => a.Id);
            modelBuilder.Entity<ResourceBooking>().HasKey(a => a.Id);
            modelBuilder.Entity<DocumentArchive>().HasKey(a => a.Id);
            modelBuilder.Entity<ArchiveLocation>().HasKey(a => a.Id);
            modelBuilder.Entity<RetentionSchedule>().HasKey(a => a.Id);
            modelBuilder.Entity<ReviewSchedule>().HasKey(a => a.Id);
            modelBuilder.Entity<AuditTrail>().HasKey(a => a.Id);
            modelBuilder.Entity<AutoClassification>().HasKey(a => a.Id);
            modelBuilder.Entity<Collaborator>().HasKey(a => a.Id);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.TenantId_Tenant).WithMany().HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.RoleId_Role).WithMany().HasForeignKey(c => c.RoleId);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.UserId_User).WithMany().HasForeignKey(c => c.UserId);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.CreatedBy_User).WithMany().HasForeignKey(c => c.CreatedBy);
            modelBuilder.Entity<UserInRole>().HasOne(a => a.UpdatedBy_User).WithMany().HasForeignKey(c => c.UpdatedBy);
            modelBuilder.Entity<UserToken>().HasOne(a => a.TenantId_Tenant).WithMany().HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<UserToken>().HasOne(a => a.UserId_User).WithMany().HasForeignKey(c => c.UserId);
            modelBuilder.Entity<RoleEntitlement>().HasOne(a => a.TenantId_Tenant).WithMany().HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<RoleEntitlement>().HasOne(a => a.RoleId_Role).WithMany().HasForeignKey(c => c.RoleId);
            modelBuilder.Entity<RoleEntitlement>().HasOne(a => a.EntityId_Entity).WithMany().HasForeignKey(c => c.EntityId);
            modelBuilder.Entity<RoleEntitlement>().HasOne(a => a.CreatedBy_User).WithMany().HasForeignKey(c => c.CreatedBy);
            modelBuilder.Entity<RoleEntitlement>().HasOne(a => a.UpdatedBy_User).WithMany().HasForeignKey(c => c.UpdatedBy);
            modelBuilder.Entity<Entity>().HasOne(a => a.TenantId_Tenant).WithMany().HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<Entity>().HasOne(a => a.CreatedBy_User).WithMany().HasForeignKey(c => c.CreatedBy);
            modelBuilder.Entity<Entity>().HasOne(a => a.UpdatedBy_User).WithMany().HasForeignKey(c => c.UpdatedBy);
            modelBuilder.Entity<User>().HasOne(a => a.TenantId_Tenant).WithMany().HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<Role>().HasOne(a => a.TenantId_Tenant).WithMany().HasForeignKey(c => c.TenantId);
            modelBuilder.Entity<Role>().HasOne(a => a.CreatedBy_User).WithMany().HasForeignKey(c => c.CreatedBy);
            modelBuilder.Entity<Role>().HasOne(a => a.UpdatedBy_User).WithMany().HasForeignKey(c => c.UpdatedBy);
            modelBuilder.Entity<ParentGuardian>().HasOne(a => a.ContactInformationId_ContactInformation).WithMany().HasForeignKey(c => c.ContactInformationId);
            modelBuilder.Entity<ParentGuardian>().HasOne(a => a.AddressId_Address).WithMany().HasForeignKey(c => c.AddressId);
            modelBuilder.Entity<Enrollment>().HasOne(a => a.StudentId_Student).WithMany().HasForeignKey(c => c.StudentId);
            modelBuilder.Entity<Enrollment>().HasOne(a => a.CourseId_Course).WithMany().HasForeignKey(c => c.CourseId);
            modelBuilder.Entity<Attendance>().HasOne(a => a.StudentId_Student).WithMany().HasForeignKey(c => c.StudentId);
            modelBuilder.Entity<Transcript>().HasOne(a => a.StudentId_Student).WithMany().HasForeignKey(c => c.StudentId);
            modelBuilder.Entity<Transcript>().HasOne(a => a.CourseId_Course).WithMany().HasForeignKey(c => c.CourseId);
            modelBuilder.Entity<Transcript>().HasOne(a => a.GradeId_Grade).WithMany().HasForeignKey(c => c.GradeId);
            modelBuilder.Entity<Assignment>().HasOne(a => a.CourseId_Course).WithMany().HasForeignKey(c => c.CourseId);
            modelBuilder.Entity<Schedule>().HasOne(a => a.CourseId_Course).WithMany().HasForeignKey(c => c.CourseId);
            modelBuilder.Entity<Fee>().HasOne(a => a.StudentId_Student).WithMany().HasForeignKey(c => c.StudentId);
            modelBuilder.Entity<Payment>().HasOne(a => a.BillingId_Billing).WithMany().HasForeignKey(c => c.BillingId);
            modelBuilder.Entity<Billing>().HasOne(a => a.DiscountId_Discount).WithMany().HasForeignKey(c => c.DiscountId);
            modelBuilder.Entity<ReportCard>().HasOne(a => a.ClassId_Class).WithMany().HasForeignKey(c => c.ClassId);
            modelBuilder.Entity<Semester>().HasOne(a => a.AcademicYearId_AcademicYear).WithMany().HasForeignKey(c => c.AcademicYearId);
            modelBuilder.Entity<Class>().HasOne(a => a.SemesterId_Semester).WithMany().HasForeignKey(c => c.SemesterId);
            modelBuilder.Entity<Classroom>().HasOne(a => a.SectionId_Section).WithMany().HasForeignKey(c => c.SectionId);
            modelBuilder.Entity<StudentDocuments>().HasOne(a => a.StudentId_Student).WithMany().HasForeignKey(c => c.StudentId);
            modelBuilder.Entity<StudentDocuments>().HasOne(a => a.DocumentTypeId_DocumentTypes).WithMany().HasForeignKey(c => c.DocumentTypeId);
            modelBuilder.Entity<TimeOffRequest>().HasOne(a => a.EmployeeId_Employee).WithMany().HasForeignKey(c => c.EmployeeId);
            modelBuilder.Entity<TimeOffRequest>().HasOne(a => a.LeaveId_Leave).WithMany().HasForeignKey(c => c.LeaveId);
            modelBuilder.Entity<PerformanceReview>().HasOne(a => a.EmployeeId_Employee).WithMany().HasForeignKey(c => c.EmployeeId);
            modelBuilder.Entity<Payroll>().HasOne(a => a.EmployeeId_Employee).WithMany().HasForeignKey(c => c.EmployeeId);
            modelBuilder.Entity<Payroll>().HasOne(a => a.SalaryId_Salary).WithMany().HasForeignKey(c => c.SalaryId);
            modelBuilder.Entity<Payroll>().HasOne(a => a.BenefitsId_Benefits).WithMany().HasForeignKey(c => c.BenefitsId);
            modelBuilder.Entity<Document>().HasOne(a => a.DocumentTypeId_DocumentType).WithMany().HasForeignKey(c => c.DocumentTypeId);
            modelBuilder.Entity<Document>().HasOne(a => a.DocumentCategoryId_DocumentCategory).WithMany().HasForeignKey(c => c.DocumentCategoryId);
            modelBuilder.Entity<Document>().HasOne(a => a.DocumentStatusId_DocumentStatus).WithMany().HasForeignKey(c => c.DocumentStatusId);
            modelBuilder.Entity<Document>().HasOne(a => a.DocumentVersionId_DocumentVersion).WithMany().HasForeignKey(c => c.DocumentVersionId);
            modelBuilder.Entity<EmergencyContact>().HasOne(a => a.StudentId_Student).WithMany().HasForeignKey(c => c.StudentId);
            modelBuilder.Entity<WorkAnnouncement>().HasOne(a => a.TeacherId_Teacher).WithMany().HasForeignKey(c => c.TeacherId);
            modelBuilder.Entity<WorkAnnouncement>().HasOne(a => a.CourseId_Course).WithMany().HasForeignKey(c => c.CourseId);
            modelBuilder.Entity<CourseSchedule>().HasOne(a => a.CourseId_Course).WithMany().HasForeignKey(c => c.CourseId);
            modelBuilder.Entity<CourseSchedule>().HasOne(a => a.RoomId_Room).WithMany().HasForeignKey(c => c.RoomId);
            modelBuilder.Entity<Room>().HasOne(a => a.BuildingId_Building).WithMany().HasForeignKey(c => c.BuildingId);
            modelBuilder.Entity<Building>().HasOne(a => a.CampusId_Campus).WithMany().HasForeignKey(c => c.CampusId);
            modelBuilder.Entity<ResourceAllocation>().HasOne(a => a.ResourceId_Resource).WithMany().HasForeignKey(c => c.ResourceId);
            modelBuilder.Entity<ResourceAllocation>().HasOne(a => a.TimeSlotId_TimeSlot).WithMany().HasForeignKey(c => c.TimeSlotId);
            modelBuilder.Entity<TimeConstraints>().HasOne(a => a.ResourceId_Resource).WithMany().HasForeignKey(c => c.ResourceId);
            modelBuilder.Entity<Resource>().HasOne(a => a.ResourceType_ResourceType).WithMany().HasForeignKey(c => c.ResourceType);
            modelBuilder.Entity<TimeLog>().HasOne(a => a.WorkScheduleId_WorkSchedule).WithMany().HasForeignKey(c => c.WorkScheduleId);
            modelBuilder.Entity<Absence>().HasOne(a => a.AbsenceTypeId_AbsenceType).WithMany().HasForeignKey(c => c.AbsenceTypeId);
            modelBuilder.Entity<TimeOff>().HasOne(a => a.TimeOffApprovalId_TimeOffApproval).WithMany().HasForeignKey(c => c.TimeOffApprovalId);
            modelBuilder.Entity<TimeOffApproval>().HasOne(a => a.TimeOffId_TimeOff).WithMany().HasForeignKey(c => c.TimeOffId);
            modelBuilder.Entity<LeaveBalance>().HasOne(a => a.EmployeeId_Employee).WithMany().HasForeignKey(c => c.EmployeeId);
            modelBuilder.Entity<LeaveBalance>().HasOne(a => a.LeaveTypeId_LeaveType).WithMany().HasForeignKey(c => c.LeaveTypeId);
            modelBuilder.Entity<AttendanceReport>().HasOne(a => a.EmployeeId_Employee).WithMany().HasForeignKey(c => c.EmployeeId);
            modelBuilder.Entity<FeeItem>().HasOne(a => a.FeeCategoryId_FeeCategory).WithMany().HasForeignKey(c => c.FeeCategoryId);
            modelBuilder.Entity<FeeSchedule>().HasOne(a => a.FeeItemId_FeeItem).WithMany().HasForeignKey(c => c.FeeItemId);
            modelBuilder.Entity<Invoice>().HasOne(a => a.CurrencyId_Currency).WithMany().HasForeignKey(c => c.CurrencyId);
            modelBuilder.Entity<Invoice>().HasOne(a => a.PaymentMethodId_PaymentMethod).WithMany().HasForeignKey(c => c.PaymentMethodId);
            modelBuilder.Entity<InvoiceLineItem>().HasOne(a => a.InvoiceId_Invoice).WithMany().HasForeignKey(c => c.InvoiceId);
            modelBuilder.Entity<InvoiceLineItem>().HasOne(a => a.FeeItemId_FeeItem).WithMany().HasForeignKey(c => c.FeeItemId);
            modelBuilder.Entity<AccountTransaction>().HasOne(a => a.FinancialAccountId_FinancialAccount).WithMany().HasForeignKey(c => c.FinancialAccountId);
            modelBuilder.Entity<AccountReconciliation>().HasOne(a => a.FinancialAccountId_FinancialAccount).WithMany().HasForeignKey(c => c.FinancialAccountId);
            modelBuilder.Entity<Installment>().HasOne(a => a.TermId_Term).WithMany().HasForeignKey(c => c.TermId);
            modelBuilder.Entity<ExamSchedule>().HasOne(a => a.ExamId_Exam).WithMany().HasForeignKey(c => c.ExamId);
            modelBuilder.Entity<ExamSchedule>().HasOne(a => a.ExamSubjectId_ExamSubject).WithMany().HasForeignKey(c => c.ExamSubjectId);
            modelBuilder.Entity<ResultDetails>().HasOne(a => a.ResultId_Result).WithMany().HasForeignKey(c => c.ResultId);
            modelBuilder.Entity<ResultDetails>().HasOne(a => a.ExamSubjectId_ExamSubject).WithMany().HasForeignKey(c => c.ExamSubjectId);
            modelBuilder.Entity<Question>().HasOne(a => a.ExamSubjectId_ExamSubject).WithMany().HasForeignKey(c => c.ExamSubjectId);
            modelBuilder.Entity<Answer>().HasOne(a => a.QuestionId_Question).WithMany().HasForeignKey(c => c.QuestionId);
            modelBuilder.Entity<MarksDistribution>().HasOne(a => a.ExamPaperId_ExamPaper).WithMany().HasForeignKey(c => c.ExamPaperId);
            modelBuilder.Entity<ExamResultNotification>().HasOne(a => a.ExamPaperId_ExamPaper).WithMany().HasForeignKey(c => c.ExamPaperId);
            modelBuilder.Entity<Folder>().HasOne(a => a.ParentFolderId_Folder).WithMany().HasForeignKey(c => c.ParentFolderId);
            modelBuilder.Entity<Message>().HasOne(a => a.SenderId_Workspace).WithMany().HasForeignKey(c => c.SenderId);
            modelBuilder.Entity<Message>().HasOne(a => a.RecipientId_Workspace).WithMany().HasForeignKey(c => c.RecipientId);
            modelBuilder.Entity<Notification>().HasOne(a => a.UserId_Workspace).WithMany().HasForeignKey(c => c.UserId);
            modelBuilder.Entity<Content>().HasOne(a => a.KnowledgeBaseId_KnowledgeBase).WithMany().HasForeignKey(c => c.KnowledgeBaseId);
            modelBuilder.Entity<Comment>().HasOne(a => a.ContentId_Content).WithMany().HasForeignKey(c => c.ContentId);
            modelBuilder.Entity<Attachment>().HasOne(a => a.ContentId_Content).WithMany().HasForeignKey(c => c.ContentId);
            modelBuilder.Entity<Agenda>().HasOne(a => a.StatusUpdateId_StatusUpdate).WithMany().HasForeignKey(c => c.StatusUpdateId);
            modelBuilder.Entity<Minutes>().HasOne(a => a.VideoConferenceId_VideoConference).WithMany().HasForeignKey(c => c.VideoConferenceId);
            modelBuilder.Entity<Share>().HasOne(a => a.ResourceId_FileStorage).WithMany().HasForeignKey(c => c.ResourceId);
            modelBuilder.Entity<Share>().HasOne(a => a.AccessLevelId_AccessLevel).WithMany().HasForeignKey(c => c.AccessLevelId);
            modelBuilder.Entity<Workflow>().HasOne(a => a.ApprovalId_Approval).WithMany().HasForeignKey(c => c.ApprovalId);
            modelBuilder.Entity<WorkflowStep>().HasOne(a => a.WorkflowId_Workflow).WithMany().HasForeignKey(c => c.WorkflowId);
            modelBuilder.Entity<ResourceBooking>().HasOne(a => a.ResourceRequestId_ResourceRequest).WithMany().HasForeignKey(c => c.ResourceRequestId);
            modelBuilder.Entity<DocumentArchive>().HasOne(a => a.ArchiveLocationId_ArchiveLocation).WithMany().HasForeignKey(c => c.ArchiveLocationId);
            modelBuilder.Entity<DocumentArchive>().HasOne(a => a.RetentionScheduleId_RetentionSchedule).WithMany().HasForeignKey(c => c.RetentionScheduleId);
            modelBuilder.Entity<DocumentArchive>().HasOne(a => a.ReviewScheduleId_ReviewSchedule).WithMany().HasForeignKey(c => c.ReviewScheduleId);
            modelBuilder.Entity<Collaborator>().HasOne(a => a.ProjectId_Project).WithMany().HasForeignKey(c => c.ProjectId);
        }

        /// <summary>
        /// Represents the database set for the UserInRole entity.
        /// </summary>
        public DbSet<UserInRole> UserInRole { get; set; }

        /// <summary>
        /// Represents the database set for the UserToken entity.
        /// </summary>
        public DbSet<UserToken> UserToken { get; set; }

        /// <summary>
        /// Represents the database set for the RoleEntitlement entity.
        /// </summary>
        public DbSet<RoleEntitlement> RoleEntitlement { get; set; }

        /// <summary>
        /// Represents the database set for the Entity entity.
        /// </summary>
        public DbSet<Entity> Entity { get; set; }

        /// <summary>
        /// Represents the database set for the Tenant entity.
        /// </summary>
        public DbSet<Tenant> Tenant { get; set; }

        /// <summary>
        /// Represents the database set for the User entity.
        /// </summary>
        public DbSet<User> User { get; set; }

        /// <summary>
        /// Represents the database set for the Role entity.
        /// </summary>
        public DbSet<Role> Role { get; set; }

        /// <summary>
        /// Represents the database set for the Student entity.
        /// </summary>
        public DbSet<Student> Student { get; set; }

        /// <summary>
        /// Represents the database set for the ParentGuardian entity.
        /// </summary>
        public DbSet<ParentGuardian> ParentGuardian { get; set; }

        /// <summary>
        /// Represents the database set for the ContactInformation entity.
        /// </summary>
        public DbSet<ContactInformation> ContactInformation { get; set; }

        /// <summary>
        /// Represents the database set for the Address entity.
        /// </summary>
        public DbSet<Address> Address { get; set; }

        /// <summary>
        /// Represents the database set for the Course entity.
        /// </summary>
        public DbSet<Course> Course { get; set; }

        /// <summary>
        /// Represents the database set for the Enrollment entity.
        /// </summary>
        public DbSet<Enrollment> Enrollment { get; set; }

        /// <summary>
        /// Represents the database set for the Attendance entity.
        /// </summary>
        public DbSet<Attendance> Attendance { get; set; }

        /// <summary>
        /// Represents the database set for the Grade entity.
        /// </summary>
        public DbSet<Grade> Grade { get; set; }

        /// <summary>
        /// Represents the database set for the Transcript entity.
        /// </summary>
        public DbSet<Transcript> Transcript { get; set; }

        /// <summary>
        /// Represents the database set for the Assignment entity.
        /// </summary>
        public DbSet<Assignment> Assignment { get; set; }

        /// <summary>
        /// Represents the database set for the Exam entity.
        /// </summary>
        public DbSet<Exam> Exam { get; set; }

        /// <summary>
        /// Represents the database set for the ExtraCurricularActivity entity.
        /// </summary>
        public DbSet<ExtraCurricularActivity> ExtraCurricularActivity { get; set; }

        /// <summary>
        /// Represents the database set for the Schedule entity.
        /// </summary>
        public DbSet<Schedule> Schedule { get; set; }

        /// <summary>
        /// Represents the database set for the Fee entity.
        /// </summary>
        public DbSet<Fee> Fee { get; set; }

        /// <summary>
        /// Represents the database set for the Payment entity.
        /// </summary>
        public DbSet<Payment> Payment { get; set; }

        /// <summary>
        /// Represents the database set for the Discount entity.
        /// </summary>
        public DbSet<Discount> Discount { get; set; }

        /// <summary>
        /// Represents the database set for the Billing entity.
        /// </summary>
        public DbSet<Billing> Billing { get; set; }

        /// <summary>
        /// Represents the database set for the ReportCard entity.
        /// </summary>
        public DbSet<ReportCard> ReportCard { get; set; }

        /// <summary>
        /// Represents the database set for the AcademicYear entity.
        /// </summary>
        public DbSet<AcademicYear> AcademicYear { get; set; }

        /// <summary>
        /// Represents the database set for the Semester entity.
        /// </summary>
        public DbSet<Semester> Semester { get; set; }

        /// <summary>
        /// Represents the database set for the Class entity.
        /// </summary>
        public DbSet<Class> Class { get; set; }

        /// <summary>
        /// Represents the database set for the Section entity.
        /// </summary>
        public DbSet<Section> Section { get; set; }

        /// <summary>
        /// Represents the database set for the Classroom entity.
        /// </summary>
        public DbSet<Classroom> Classroom { get; set; }

        /// <summary>
        /// Represents the database set for the Faculty entity.
        /// </summary>
        public DbSet<Faculty> Faculty { get; set; }

        /// <summary>
        /// Represents the database set for the Staff entity.
        /// </summary>
        public DbSet<Staff> Staff { get; set; }

        /// <summary>
        /// Represents the database set for the NoticeBoard entity.
        /// </summary>
        public DbSet<NoticeBoard> NoticeBoard { get; set; }

        /// <summary>
        /// Represents the database set for the SchoolEvents entity.
        /// </summary>
        public DbSet<SchoolEvents> SchoolEvents { get; set; }

        /// <summary>
        /// Represents the database set for the Library entity.
        /// </summary>
        public DbSet<Library> Library { get; set; }

        /// <summary>
        /// Represents the database set for the DocumentTypes entity.
        /// </summary>
        public DbSet<DocumentTypes> DocumentTypes { get; set; }

        /// <summary>
        /// Represents the database set for the StudentDocuments entity.
        /// </summary>
        public DbSet<StudentDocuments> StudentDocuments { get; set; }

        /// <summary>
        /// Represents the database set for the Employee entity.
        /// </summary>
        public DbSet<Employee> Employee { get; set; }

        /// <summary>
        /// Represents the database set for the Department entity.
        /// </summary>
        public DbSet<Department> Department { get; set; }

        /// <summary>
        /// Represents the database set for the JobTitle entity.
        /// </summary>
        public DbSet<JobTitle> JobTitle { get; set; }

        /// <summary>
        /// Represents the database set for the EmploymentStatus entity.
        /// </summary>
        public DbSet<EmploymentStatus> EmploymentStatus { get; set; }

        /// <summary>
        /// Represents the database set for the Shift entity.
        /// </summary>
        public DbSet<Shift> Shift { get; set; }

        /// <summary>
        /// Represents the database set for the TimeOffRequest entity.
        /// </summary>
        public DbSet<TimeOffRequest> TimeOffRequest { get; set; }

        /// <summary>
        /// Represents the database set for the Leave entity.
        /// </summary>
        public DbSet<Leave> Leave { get; set; }

        /// <summary>
        /// Represents the database set for the PerformanceReview entity.
        /// </summary>
        public DbSet<PerformanceReview> PerformanceReview { get; set; }

        /// <summary>
        /// Represents the database set for the Payroll entity.
        /// </summary>
        public DbSet<Payroll> Payroll { get; set; }

        /// <summary>
        /// Represents the database set for the Salary entity.
        /// </summary>
        public DbSet<Salary> Salary { get; set; }

        /// <summary>
        /// Represents the database set for the Benefits entity.
        /// </summary>
        public DbSet<Benefits> Benefits { get; set; }

        /// <summary>
        /// Represents the database set for the Skill entity.
        /// </summary>
        public DbSet<Skill> Skill { get; set; }

        /// <summary>
        /// Represents the database set for the Training entity.
        /// </summary>
        public DbSet<Training> Training { get; set; }

        /// <summary>
        /// Represents the database set for the Document entity.
        /// </summary>
        public DbSet<Document> Document { get; set; }

        /// <summary>
        /// Represents the database set for the EmergencyContact entity.
        /// </summary>
        public DbSet<EmergencyContact> EmergencyContact { get; set; }

        /// <summary>
        /// Represents the database set for the WorkAnnouncement entity.
        /// </summary>
        public DbSet<WorkAnnouncement> WorkAnnouncement { get; set; }

        /// <summary>
        /// Represents the database set for the Teacher entity.
        /// </summary>
        public DbSet<Teacher> Teacher { get; set; }

        /// <summary>
        /// Represents the database set for the Students entity.
        /// </summary>
        public DbSet<Students> Students { get; set; }

        /// <summary>
        /// Represents the database set for the CourseSchedule entity.
        /// </summary>
        public DbSet<CourseSchedule> CourseSchedule { get; set; }

        /// <summary>
        /// Represents the database set for the Instructor entity.
        /// </summary>
        public DbSet<Instructor> Instructor { get; set; }

        /// <summary>
        /// Represents the database set for the Room entity.
        /// </summary>
        public DbSet<Room> Room { get; set; }

        /// <summary>
        /// Represents the database set for the Building entity.
        /// </summary>
        public DbSet<Building> Building { get; set; }

        /// <summary>
        /// Represents the database set for the Campus entity.
        /// </summary>
        public DbSet<Campus> Campus { get; set; }

        /// <summary>
        /// Represents the database set for the AcademicSession entity.
        /// </summary>
        public DbSet<AcademicSession> AcademicSession { get; set; }

        /// <summary>
        /// Represents the database set for the ClassPeriod entity.
        /// </summary>
        public DbSet<ClassPeriod> ClassPeriod { get; set; }

        /// <summary>
        /// Represents the database set for the TimetableTemplate entity.
        /// </summary>
        public DbSet<TimetableTemplate> TimetableTemplate { get; set; }

        /// <summary>
        /// Represents the database set for the Holidays entity.
        /// </summary>
        public DbSet<Holidays> Holidays { get; set; }

        /// <summary>
        /// Represents the database set for the Breaks entity.
        /// </summary>
        public DbSet<Breaks> Breaks { get; set; }

        /// <summary>
        /// Represents the database set for the ClassType entity.
        /// </summary>
        public DbSet<ClassType> ClassType { get; set; }

        /// <summary>
        /// Represents the database set for the ConflictResolution entity.
        /// </summary>
        public DbSet<ConflictResolution> ConflictResolution { get; set; }

        /// <summary>
        /// Represents the database set for the MeetingPattern entity.
        /// </summary>
        public DbSet<MeetingPattern> MeetingPattern { get; set; }

        /// <summary>
        /// Represents the database set for the ResourceAllocation entity.
        /// </summary>
        public DbSet<ResourceAllocation> ResourceAllocation { get; set; }

        /// <summary>
        /// Represents the database set for the TimeSlot entity.
        /// </summary>
        public DbSet<TimeSlot> TimeSlot { get; set; }

        /// <summary>
        /// Represents the database set for the TimeConstraints entity.
        /// </summary>
        public DbSet<TimeConstraints> TimeConstraints { get; set; }

        /// <summary>
        /// Represents the database set for the Resource entity.
        /// </summary>
        public DbSet<Resource> Resource { get; set; }

        /// <summary>
        /// Represents the database set for the TimeLog entity.
        /// </summary>
        public DbSet<TimeLog> TimeLog { get; set; }

        /// <summary>
        /// Represents the database set for the WorkSchedule entity.
        /// </summary>
        public DbSet<WorkSchedule> WorkSchedule { get; set; }

        /// <summary>
        /// Represents the database set for the Absence entity.
        /// </summary>
        public DbSet<Absence> Absence { get; set; }

        /// <summary>
        /// Represents the database set for the AbsenceType entity.
        /// </summary>
        public DbSet<AbsenceType> AbsenceType { get; set; }

        /// <summary>
        /// Represents the database set for the Overtime entity.
        /// </summary>
        public DbSet<Overtime> Overtime { get; set; }

        /// <summary>
        /// Represents the database set for the TimeOff entity.
        /// </summary>
        public DbSet<TimeOff> TimeOff { get; set; }

        /// <summary>
        /// Represents the database set for the TimeOffApproval entity.
        /// </summary>
        public DbSet<TimeOffApproval> TimeOffApproval { get; set; }

        /// <summary>
        /// Represents the database set for the LeaveBalance entity.
        /// </summary>
        public DbSet<LeaveBalance> LeaveBalance { get; set; }

        /// <summary>
        /// Represents the database set for the PayPeriod entity.
        /// </summary>
        public DbSet<PayPeriod> PayPeriod { get; set; }

        /// <summary>
        /// Represents the database set for the AttendanceReport entity.
        /// </summary>
        public DbSet<AttendanceReport> AttendanceReport { get; set; }

        /// <summary>
        /// Represents the database set for the AttendancePolicy entity.
        /// </summary>
        public DbSet<AttendancePolicy> AttendancePolicy { get; set; }

        /// <summary>
        /// Represents the database set for the LeaveType entity.
        /// </summary>
        public DbSet<LeaveType> LeaveType { get; set; }

        /// <summary>
        /// Represents the database set for the FeeCategory entity.
        /// </summary>
        public DbSet<FeeCategory> FeeCategory { get; set; }

        /// <summary>
        /// Represents the database set for the FeeItem entity.
        /// </summary>
        public DbSet<FeeItem> FeeItem { get; set; }

        /// <summary>
        /// Represents the database set for the FeeSchedule entity.
        /// </summary>
        public DbSet<FeeSchedule> FeeSchedule { get; set; }

        /// <summary>
        /// Represents the database set for the Invoice entity.
        /// </summary>
        public DbSet<Invoice> Invoice { get; set; }

        /// <summary>
        /// Represents the database set for the InvoiceLineItem entity.
        /// </summary>
        public DbSet<InvoiceLineItem> InvoiceLineItem { get; set; }

        /// <summary>
        /// Represents the database set for the PaymentMethod entity.
        /// </summary>
        public DbSet<PaymentMethod> PaymentMethod { get; set; }

        /// <summary>
        /// Represents the database set for the Currency entity.
        /// </summary>
        public DbSet<Currency> Currency { get; set; }

        /// <summary>
        /// Represents the database set for the Tax entity.
        /// </summary>
        public DbSet<Tax> Tax { get; set; }

        /// <summary>
        /// Represents the database set for the BillingFrequency entity.
        /// </summary>
        public DbSet<BillingFrequency> BillingFrequency { get; set; }

        /// <summary>
        /// Represents the database set for the FinancialAccount entity.
        /// </summary>
        public DbSet<FinancialAccount> FinancialAccount { get; set; }

        /// <summary>
        /// Represents the database set for the AccountTransaction entity.
        /// </summary>
        public DbSet<AccountTransaction> AccountTransaction { get; set; }

        /// <summary>
        /// Represents the database set for the AccountReconciliation entity.
        /// </summary>
        public DbSet<AccountReconciliation> AccountReconciliation { get; set; }

        /// <summary>
        /// Represents the database set for the PaymentGateway entity.
        /// </summary>
        public DbSet<PaymentGateway> PaymentGateway { get; set; }

        /// <summary>
        /// Represents the database set for the PaymentStatus entity.
        /// </summary>
        public DbSet<PaymentStatus> PaymentStatus { get; set; }

        /// <summary>
        /// Represents the database set for the LateFee entity.
        /// </summary>
        public DbSet<LateFee> LateFee { get; set; }

        /// <summary>
        /// Represents the database set for the Refund entity.
        /// </summary>
        public DbSet<Refund> Refund { get; set; }

        /// <summary>
        /// Represents the database set for the PaymentTerms entity.
        /// </summary>
        public DbSet<PaymentTerms> PaymentTerms { get; set; }

        /// <summary>
        /// Represents the database set for the BillingCycle entity.
        /// </summary>
        public DbSet<BillingCycle> BillingCycle { get; set; }

        /// <summary>
        /// Represents the database set for the FeeAdjustment entity.
        /// </summary>
        public DbSet<FeeAdjustment> FeeAdjustment { get; set; }

        /// <summary>
        /// Represents the database set for the School entity.
        /// </summary>
        public DbSet<School> School { get; set; }

        /// <summary>
        /// Represents the database set for the Term entity.
        /// </summary>
        public DbSet<Term> Term { get; set; }

        /// <summary>
        /// Represents the database set for the Installment entity.
        /// </summary>
        public DbSet<Installment> Installment { get; set; }

        /// <summary>
        /// Represents the database set for the FeeWaiver entity.
        /// </summary>
        public DbSet<FeeWaiver> FeeWaiver { get; set; }

        /// <summary>
        /// Represents the database set for the ExamSchedule entity.
        /// </summary>
        public DbSet<ExamSchedule> ExamSchedule { get; set; }

        /// <summary>
        /// Represents the database set for the Result entity.
        /// </summary>
        public DbSet<Result> Result { get; set; }

        /// <summary>
        /// Represents the database set for the ResultDetails entity.
        /// </summary>
        public DbSet<ResultDetails> ResultDetails { get; set; }

        /// <summary>
        /// Represents the database set for the ExamSubject entity.
        /// </summary>
        public DbSet<ExamSubject> ExamSubject { get; set; }

        /// <summary>
        /// Represents the database set for the Question entity.
        /// </summary>
        public DbSet<Question> Question { get; set; }

        /// <summary>
        /// Represents the database set for the Answer entity.
        /// </summary>
        public DbSet<Answer> Answer { get; set; }

        /// <summary>
        /// Represents the database set for the QuestionType entity.
        /// </summary>
        public DbSet<QuestionType> QuestionType { get; set; }

        /// <summary>
        /// Represents the database set for the QuestionCategory entity.
        /// </summary>
        public DbSet<QuestionCategory> QuestionCategory { get; set; }

        /// <summary>
        /// Represents the database set for the ExamRoom entity.
        /// </summary>
        public DbSet<ExamRoom> ExamRoom { get; set; }

        /// <summary>
        /// Represents the database set for the GradingScale entity.
        /// </summary>
        public DbSet<GradingScale> GradingScale { get; set; }

        /// <summary>
        /// Represents the database set for the Examiner entity.
        /// </summary>
        public DbSet<Examiner> Examiner { get; set; }

        /// <summary>
        /// Represents the database set for the ExaminationBoard entity.
        /// </summary>
        public DbSet<ExaminationBoard> ExaminationBoard { get; set; }

        /// <summary>
        /// Represents the database set for the ExamSession entity.
        /// </summary>
        public DbSet<ExamSession> ExamSession { get; set; }

        /// <summary>
        /// Represents the database set for the ExamPaper entity.
        /// </summary>
        public DbSet<ExamPaper> ExamPaper { get; set; }

        /// <summary>
        /// Represents the database set for the MarksDistribution entity.
        /// </summary>
        public DbSet<MarksDistribution> MarksDistribution { get; set; }

        /// <summary>
        /// Represents the database set for the AttendanceStatus entity.
        /// </summary>
        public DbSet<AttendanceStatus> AttendanceStatus { get; set; }

        /// <summary>
        /// Represents the database set for the Certificate entity.
        /// </summary>
        public DbSet<Certificate> Certificate { get; set; }

        /// <summary>
        /// Represents the database set for the ExamResultNotification entity.
        /// </summary>
        public DbSet<ExamResultNotification> ExamResultNotification { get; set; }

        /// <summary>
        /// Represents the database set for the EvaluationCriteria entity.
        /// </summary>
        public DbSet<EvaluationCriteria> EvaluationCriteria { get; set; }

        /// <summary>
        /// Represents the database set for the ExamResultTemplate entity.
        /// </summary>
        public DbSet<ExamResultTemplate> ExamResultTemplate { get; set; }

        /// <summary>
        /// Represents the database set for the Email entity.
        /// </summary>
        public DbSet<Email> Email { get; set; }

        /// <summary>
        /// Represents the database set for the Chat entity.
        /// </summary>
        public DbSet<Chat> Chat { get; set; }

        /// <summary>
        /// Represents the database set for the Calendar entity.
        /// </summary>
        public DbSet<Calendar> Calendar { get; set; }

        /// <summary>
        /// Represents the database set for the Meeting entity.
        /// </summary>
        public DbSet<Meeting> Meeting { get; set; }

        /// <summary>
        /// Represents the database set for the Event entity.
        /// </summary>
        public DbSet<Event> Event { get; set; }

        /// <summary>
        /// Represents the database set for the Contact entity.
        /// </summary>
        public DbSet<Contact> Contact { get; set; }

        /// <summary>
        /// Represents the database set for the Folder entity.
        /// </summary>
        public DbSet<Folder> Folder { get; set; }

        /// <summary>
        /// Represents the database set for the Message entity.
        /// </summary>
        public DbSet<Message> Message { get; set; }

        /// <summary>
        /// Represents the database set for the Notification entity.
        /// </summary>
        public DbSet<Notification> Notification { get; set; }

        /// <summary>
        /// Represents the database set for the Project entity.
        /// </summary>
        public DbSet<Project> Project { get; set; }

        /// <summary>
        /// Represents the database set for the Workspace entity.
        /// </summary>
        public DbSet<Workspace> Workspace { get; set; }

        /// <summary>
        /// Represents the database set for the Content entity.
        /// </summary>
        public DbSet<Content> Content { get; set; }

        /// <summary>
        /// Represents the database set for the KnowledgeBase entity.
        /// </summary>
        public DbSet<KnowledgeBase> KnowledgeBase { get; set; }

        /// <summary>
        /// Represents the database set for the Comment entity.
        /// </summary>
        public DbSet<Comment> Comment { get; set; }

        /// <summary>
        /// Represents the database set for the Poll entity.
        /// </summary>
        public DbSet<Poll> Poll { get; set; }

        /// <summary>
        /// Represents the database set for the Survey entity.
        /// </summary>
        public DbSet<Survey> Survey { get; set; }

        /// <summary>
        /// Represents the database set for the Attachment entity.
        /// </summary>
        public DbSet<Attachment> Attachment { get; set; }

        /// <summary>
        /// Represents the database set for the Channel entity.
        /// </summary>
        public DbSet<Channel> Channel { get; set; }

        /// <summary>
        /// Represents the database set for the Discussion entity.
        /// </summary>
        public DbSet<Discussion> Discussion { get; set; }

        /// <summary>
        /// Represents the database set for the VideoConference entity.
        /// </summary>
        public DbSet<VideoConference> VideoConference { get; set; }

        /// <summary>
        /// Represents the database set for the Agenda entity.
        /// </summary>
        public DbSet<Agenda> Agenda { get; set; }

        /// <summary>
        /// Represents the database set for the Minutes entity.
        /// </summary>
        public DbSet<Minutes> Minutes { get; set; }

        /// <summary>
        /// Represents the database set for the StatusUpdate entity.
        /// </summary>
        public DbSet<StatusUpdate> StatusUpdate { get; set; }

        /// <summary>
        /// Represents the database set for the ActivityFeed entity.
        /// </summary>
        public DbSet<ActivityFeed> ActivityFeed { get; set; }

        /// <summary>
        /// Represents the database set for the Tag entity.
        /// </summary>
        public DbSet<Tag> Tag { get; set; }

        /// <summary>
        /// Represents the database set for the Category entity.
        /// </summary>
        public DbSet<Category> Category { get; set; }

        /// <summary>
        /// Represents the database set for the Versioning entity.
        /// </summary>
        public DbSet<Versioning> Versioning { get; set; }

        /// <summary>
        /// Represents the database set for the DocumentType entity.
        /// </summary>
        public DbSet<DocumentType> DocumentType { get; set; }

        /// <summary>
        /// Represents the database set for the DocumentCategory entity.
        /// </summary>
        public DbSet<DocumentCategory> DocumentCategory { get; set; }

        /// <summary>
        /// Represents the database set for the DocumentStatus entity.
        /// </summary>
        public DbSet<DocumentStatus> DocumentStatus { get; set; }

        /// <summary>
        /// Represents the database set for the DocumentVersion entity.
        /// </summary>
        public DbSet<DocumentVersion> DocumentVersion { get; set; }

        /// <summary>
        /// Represents the database set for the ResourceType entity.
        /// </summary>
        public DbSet<ResourceType> ResourceType { get; set; }

        /// <summary>
        /// Represents the database set for the ResourceCategory entity.
        /// </summary>
        public DbSet<ResourceCategory> ResourceCategory { get; set; }

        /// <summary>
        /// Represents the database set for the ResourceStatus entity.
        /// </summary>
        public DbSet<ResourceStatus> ResourceStatus { get; set; }

        /// <summary>
        /// Represents the database set for the AccessLevel entity.
        /// </summary>
        public DbSet<AccessLevel> AccessLevel { get; set; }

        /// <summary>
        /// Represents the database set for the Tags entity.
        /// </summary>
        public DbSet<Tags> Tags { get; set; }

        /// <summary>
        /// Represents the database set for the FileStorage entity.
        /// </summary>
        public DbSet<FileStorage> FileStorage { get; set; }

        /// <summary>
        /// Represents the database set for the Share entity.
        /// </summary>
        public DbSet<Share> Share { get; set; }

        /// <summary>
        /// Represents the database set for the History entity.
        /// </summary>
        public DbSet<History> History { get; set; }

        /// <summary>
        /// Represents the database set for the Permissions entity.
        /// </summary>
        public DbSet<Permissions> Permissions { get; set; }

        /// <summary>
        /// Represents the database set for the Workflow entity.
        /// </summary>
        public DbSet<Workflow> Workflow { get; set; }

        /// <summary>
        /// Represents the database set for the Approval entity.
        /// </summary>
        public DbSet<Approval> Approval { get; set; }

        /// <summary>
        /// Represents the database set for the WorkflowStep entity.
        /// </summary>
        public DbSet<WorkflowStep> WorkflowStep { get; set; }

        /// <summary>
        /// Represents the database set for the StorageProvider entity.
        /// </summary>
        public DbSet<StorageProvider> StorageProvider { get; set; }

        /// <summary>
        /// Represents the database set for the SearchCriteria entity.
        /// </summary>
        public DbSet<SearchCriteria> SearchCriteria { get; set; }

        /// <summary>
        /// Represents the database set for the DocumentTemplate entity.
        /// </summary>
        public DbSet<DocumentTemplate> DocumentTemplate { get; set; }

        /// <summary>
        /// Represents the database set for the ResourceRequest entity.
        /// </summary>
        public DbSet<ResourceRequest> ResourceRequest { get; set; }

        /// <summary>
        /// Represents the database set for the ResourceBooking entity.
        /// </summary>
        public DbSet<ResourceBooking> ResourceBooking { get; set; }

        /// <summary>
        /// Represents the database set for the DocumentArchive entity.
        /// </summary>
        public DbSet<DocumentArchive> DocumentArchive { get; set; }

        /// <summary>
        /// Represents the database set for the ArchiveLocation entity.
        /// </summary>
        public DbSet<ArchiveLocation> ArchiveLocation { get; set; }

        /// <summary>
        /// Represents the database set for the RetentionSchedule entity.
        /// </summary>
        public DbSet<RetentionSchedule> RetentionSchedule { get; set; }

        /// <summary>
        /// Represents the database set for the ReviewSchedule entity.
        /// </summary>
        public DbSet<ReviewSchedule> ReviewSchedule { get; set; }

        /// <summary>
        /// Represents the database set for the AuditTrail entity.
        /// </summary>
        public DbSet<AuditTrail> AuditTrail { get; set; }

        /// <summary>
        /// Represents the database set for the AutoClassification entity.
        /// </summary>
        public DbSet<AutoClassification> AutoClassification { get; set; }

        /// <summary>
        /// Represents the database set for the Collaborator entity.
        /// </summary>
        public DbSet<Collaborator> Collaborator { get; set; }
    }
}