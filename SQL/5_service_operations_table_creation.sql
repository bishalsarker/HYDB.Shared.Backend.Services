create table ServiceOperations (
	Id varchar(900) not null primary key,
	Name varchar(max) not null,
	Type varchar(max) not null,
	ServiceId varchar(900) foreign key references DataServices(Id)
);

insert into MigrationHistory(ScriptName) values('5_service_operations_table_creation');