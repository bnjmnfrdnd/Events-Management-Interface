# Events Management Interface

##What is the purpose of the application?
The purpose of this application is to provide management staff with a simple interface to view and manage guest activity at a corporate event. This application also provides vendors at the event with a POS-style interface when interacting with guests.

##Why was this application made?
A client required this application to track expenditure of each guest at their event. This allows the client to see in real time, the total expense per guest and forecast future spending for similar events. The client pre-paid for vendors at the event and required a figure on how much is being spent per vendor and per guest and if they have overspent/underspent on the event vendors. There were two types of vendors - drink or food.

##How is this implemented?
A week prior to the event the client is provided access to the application. From here the staff are able to upload a CSV of invitees to the event onto the application. The CSV data provided is required in the following format:

EmailAddress,FirstName,LastName,AlcoholicDrinkTokenAllowance,NonAlcoholicDrinkTokenAllowance,FoodTokenAllowance

The application stores this data to a MSSQL Database and provides each row (guest) on the CSV with a randomly assigned 4-digit pin e.g. 7364. This pin is then associated with the guest stored in the database (UMLs to come later). Once the data is loaded in and the staff are satisfied with the guest data in the application, an email can be sent out from the admin page to each provided guest email address, with details of the event and how to use their 4-digit pin. 

##What’s the purpose of the 4-digit pin?
The purpose of the 4-digit pin is to control how much guests would go to the prepaid event vendors and to reduce the chance of exceeding the prepaid amount. Each guest receives ‘tokens’ associated with their pin. There are three types of tokens:

 - Alcoholic drink
 - Non-alcoholic drink
 - Food

For a guest bringing 2 children and 1 adult, the tokens would accommodate the requirements for example, 4 alcoholic drink tokens, 8 non-alcoholic, 4 food tokens. The token total for each guest was provided by the client and uploaded in the CSV.

##How is guest activity tracked?
The application also contains a POS-style page for the vendors. When a guest approaches a vendor, the vendor will enter their 4-digit pin into the page and a subsequent page is shown with the vendors menu. From here they can select what the user wants, alcoholic drink, non-alcoholic-drink, food,  and after the transaction is made, the data is stored in the database. The vendor’s menu items also include pricing. So the vendor can take away their earnings of the day from a report that can be generated from the data in the management interface of the application. With the data being this granular it is possible for the staff to see in real time the transactions throughout the day and see a selection of data provided in charts and graphs such as:

 - Most token type spent
 - What the most popular vendor menu item was
 - Which guest used how many tokens
 - Most popular time of day for transactions
 - Total tokens spent
 - Total spent

##Known bugs
1. When creating the unique 4-digit pin for each user upon uploading data. The data is stored in a local list. Whilst this list is verified against current data in the database to ensure there are no duplicate pins, the code does not ensure that each pin entered into the list is unique against the currently stored pins in the same list. This resulted in about 3 guests having the same pin, which caused an exception to be thrown as the application was unsure of which guest the 4-digit pin was meant to be assigned to. This was temporarily fixed on the day by providing an ‘override’ pin (0000) which the vendor could use if they encounter any of these guests - the override pin was assigned with unlimited tokens.



