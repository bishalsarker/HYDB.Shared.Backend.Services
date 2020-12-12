create table DataObjectKeyValues (
	DataObjectId varchar(900) foreign key references DataObject(Id),
	KeyString varchar(max),
	Value varchar(max)
);

insert into MigrationHistory(ScriptName) values('7_data_object_key_value_table_creation');