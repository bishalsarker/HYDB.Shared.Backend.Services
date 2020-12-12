create table DataObject (
	Id varchar(900) not null primary key,
	ReferenceId varchar(max) not null,
);

insert into MigrationHistory(ScriptName) values('6_data_object_table_creation');