create table Clients (
	Id varchar(900) not null primary key,
	Name varchar(max) not null,
	ApiKey varchar(max) not null,
	CreatedBy varchar(900) foreign key references UserAccounts(Id) not null
);

insert into MigrationHistory(ScriptName) values('8_clients_table_creation');