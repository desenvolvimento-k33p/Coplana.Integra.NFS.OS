	 create schema KEEPLOGS;
	 
	 create column table KEEPLOGS."KEEP_LOG_NFS_OS"( "LOGDATE" DATE null,
	 "LOGHOUR" TIME null,	 
	 "COMPANY" VARCHAR (254) null,	 
	 "MESSAGE" VARCHAR (254) null,
	 "KEY_PARC" VARCHAR (254) null,
	 "KEY_SAP" VARCHAR (254) null,
	 "REQUESTOBJECT" TEXT null,
	 "RESPONSEOBJECT" TEXT null,
	 "OWNER" VARCHAR (254) null,
	 "METHOD" VARCHAR (254) null);