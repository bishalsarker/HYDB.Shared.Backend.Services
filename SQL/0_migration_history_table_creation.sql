create table MigrationHistory (
	Id int identity(1,1) not null primary key,
	ScriptName varchar(max) not null
);