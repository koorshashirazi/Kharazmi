
namespace Kharazmi.AspNetCore.Core.IntegrationTests.Settings
{
    /// <summary>
    /// All static javaScript attributes
    /// </summary>
    public static class JsConstraint
    {
        public const string UpdateTargetKey = "UpdateTargetId";
        public static string AntiToken = "__RequestVerificationToken";

        public const string GeneralUserProcessSearchFormId = nameof(GeneralUserProcessSearchFormId);
        public const string GeneralCompletedUserProcessSearchFormId = nameof(GeneralCompletedUserProcessSearchFormId);
        public const string GeneralSearchTextBoxId = "general_Search_textBox_id";
        public const string InProgressProcessContainerId = "in_progress_process_container_id";
        public const string CompletedProgressProcessContainerId = "completed_progress_process_container_id";

        public const string StepsOptionsButtonId = nameof(StepsOptionsButtonId);
        public const string StepsOptionsFormId = nameof(StepsOptionsFormId);
        public const string StepsOptionsStepContainerId = nameof(StepsOptionsStepContainerId);

        public const string ProcessJsOptionsFormId = nameof(ProcessJsOptionsFormId);
        public const string ProcessJsOptionsButtonId = nameof(ProcessJsOptionsButtonId);

        public const string AjaxOptionsSuccessFunction = "ajaxResponseResult";

        public const string RenderBodyContainerId = nameof(RenderBodyContainerId);

        public const string EditorConfigOptionsFormId = nameof(EditorConfigOptionsFormId);

        public const string DocUploadOptionsUploadElementId = nameof(DocUploadOptionsUploadElementId);

        public const string LeftAsideSecondaryMenuHomeTabId = "kt_aside_tab_1";
        public const string LeftAsideSecondaryMenuInProcessTabId = "kt_aside_tab_2";
        public const string LeftAsideSecondaryMenuCompletedProcessTabId = "kt_aside_tab_3";

        public const string UserPanelOptionsUserPanelId = "kt_quick_user";
        public const string UserPanelOptionsUserPanelInfoId = "kt_quick_user_Info";

        public const string RightPanelActiveUserProcessPanelId = "active_user_process_right_panel_id";
        public const string MenuCreateProcessButtonId = nameof(MenuCreateProcessButtonId);
        public const string MenuCreateProcessModalId = nameof(MenuCreateProcessModalId);
        public const string MenuDeleteProcessContainerId = nameof(MenuDeleteProcessContainerId);
        public const string MenuDeleteProcessButtonId = nameof(MenuDeleteProcessButtonId);
        public const string MenuDeleteProcessModalId = nameof(MenuDeleteProcessModalId);

        public const string DashboardLineChartContainerId = nameof(DashboardLineChartContainerId);
        public const string DashboardDonutChartContainerId = nameof(DashboardDonutChartContainerId);
        public const string DashboardTableContainerId = nameof(DashboardTableContainerId);

        public const string SearchEditorContainerId = nameof(SearchEditorContainerId);
        public const string SelectEditorId = nameof(SelectEditorId);
    }
}