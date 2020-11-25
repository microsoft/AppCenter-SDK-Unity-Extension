## Release 3.0.0

* **[Fix]** Fix version check date time value parsing. 
* **[Improvement]** Detect `AppCenterEditorExtensions` location automatically.

### App Center Push

App Center Push has been removed from the SDK and will be [retired on December 31st, 2020](https://devblogs.microsoft.com/appcenter/migrating-off-app-center-push/).  As an alternative to App Center Push, we recommend you migrate to [Azure Notification Hubs](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-overview) by following the [Push Migration Guide](https://docs.microsoft.com/en-us/appcenter/migration/push/).

## Release 2.0.0

* **[Breaking Change]** App Center Auth is [retired](https://aka.ms/MBaaS-retirement-blog-post) and has been removed from the SDK. Removed Auth package support.
* **[Bug fix]** Fixed excessive amount of GitHub API calls.