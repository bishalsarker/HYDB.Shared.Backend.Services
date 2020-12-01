create table DataServices (
	Id varchar(900) not null primary key,
	Name varchar(max) not null,
	CreatedBy varchar(900) foreign key references UserAccounts(Id) not null
);

insert into MigrationHistory(ScriptName) values('4_data_services_table_creation');