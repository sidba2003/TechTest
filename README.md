# Project Completion ReadMe (Siddharth Bahl)

Please go through this section to understand a bit more about the work I have done.

## Sections Completed

I have completed all the tasks within the **Standard** and the **Advanced** sections, along with two tasks of the **Expert** section. And I have also converted everything to async.

The two tasks I have done as part of the **Expert** section are:

1.> **re-implement the UI using a client side library** and have it interact with API endpoints, I have implemented the same in React (and the same can be found under react_app in wwwroot), and the interactions with the backend is through endpoints. 

2.> **Implement the bundling of static assets**, the front end is automatically bundled when the project is built, the user just has to build and run the project, and go to root path "/"

For the **advanced section** tasks, the operations against an user and throughout the application is displayed through a common component (only the data passed to the component is different in each case).

I have implemented the options to filter the operations by the type of HTTP request and pagination (you can see 5, 10 and 20 operations at once).

A potential enhancement in this case could have been to implement options for even more filters (such as by email, user id, etc.)

But due to the time constraints, I decided against it.

## Further Enhancements

I have added the unit tests for the backend, although my frontend doesnt have any testing which can be re-run (as I primarily relied on manual testing for the same), but I believe the implementation unit tests for the front end and E2E tests for the full stack would have been nice (using libraries such as Playwright).

## Additional Tools

**npm** will need to be installed, as the project automatically installs and buils the react_app whenever the project is built.

# User Management Technical Exercise

The exercise is an ASP.NET Core web application backed by Entity Framework Core, which faciliates management of some fictional users.
We recommend that you use [Visual Studio (Community Edition)](https://visualstudio.microsoft.com/downloads) or [Visual Studio Code](https://code.visualstudio.com/Download) to run and modify the application. 

**The application uses an in-memory database, so changes will not be persisted between executions.**

## The Exercise
Complete as many of the tasks below as you feel comfortable with. These are split into 4 levels of difficulty 
* **Standard** - Functionality that is common when working as a web developer
* **Advanced** - Slightly more technical tasks and problem solving
* **Expert** - Tasks with a higher level of problem solving and architecture needed
* **Platform** - Tasks with a focus on infrastructure and scaleability, rather than application development.

### 1. Filters Section (Standard)

The users page contains 3 buttons below the user listing - **Show All**, **Active Only** and **Non Active**. Show All has already been implemented. Implement the remaining buttons using the following logic:
* Active Only – This should show only users where their `IsActive` property is set to `true`
* Non Active – This should show only users where their `IsActive` property is set to `false`

### 2. User Model Properties (Standard)

Add a new property to the `User` class in the system called `DateOfBirth` which is to be used and displayed in relevant sections of the app.

### 3. Actions Section (Standard)

Create the code and UI flows for the following actions
* **Add** – A screen that allows you to create a new user and return to the list
* **View** - A screen that displays the information about a user
* **Edit** – A screen that allows you to edit a selected user from the list  
* **Delete** – A screen that allows you to delete a selected user from the list

Each of these screens should contain appropriate data validation, which is communicated to the end user.

### 4. Data Logging (Advanced)

Extend the system to capture log information regarding primary actions performed on each user in the app.
* In the **View** screen there should be a list of all actions that have been performed against that user. 
* There should be a new **Logs** page, containing a list of log entries across the application.
* In the Logs page, the user should be able to click into each entry to see more detail about it.
* In the Logs page, think about how you can provide a good user experience - even when there are many log entries.

### 5. Extend the Application (Expert)

Make a significant architectural change that improves the application.
Structurally, the user management application is very simple, and there are many ways it can be made more maintainable, scalable or testable.
Some ideas are:
* Re-implement the UI using a client side framework connecting to an API. Use of Blazor is preferred, but if you are more familiar with other frameworks, feel free to use them.
* Update the data access layer to support asynchronous operations.
* Implement authentication and login based on the users being stored.
* Implement bundling of static assets.
* Update the data access layer to use a real database, and implement database schema migrations.

### 6. Future-Proof the Application (Platform)

Add additional layers to the application that will ensure that it is scaleable with many users or developers. For example:
* Add CI pipelines to run tests and build the application.
* Add CD pipelines to deploy the application to cloud infrastructure.
* Add IaC to support easy deployment to new environments.
* Introduce a message bus and/or worker to handle long-running operations.

## Additional Notes

* Please feel free to change or refactor any code that has been supplied within the solution and think about clean maintainable code and architecture when extending the project.
* If any additional packages, tools or setup are required to run your completed version, please document these thoroughly.
