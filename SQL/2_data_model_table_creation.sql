create table DataModels (
	Id varchar(900) not null primary key,
	Name varchar(max) not null,
	CreatedBy varchar(900) foreign key references UserAccounts(Id) not null
);

insert into MigrationHistory(ScriptName) values('2_data_model_table_creation');