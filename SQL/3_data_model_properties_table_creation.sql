create table DataModelProperties (
	Id varchar(900) not null primary key,
	Name varchar(max) not null,
	Type varchar(max) not null,
	DataModelId varchar(900) foreign key references DataModels(Id)
);

insert into MigrationHistory(ScriptName) values('3_data_model_properties_table_creation');