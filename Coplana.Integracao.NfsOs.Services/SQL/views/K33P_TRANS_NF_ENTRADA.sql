CREATE VIEW "COPLANA_QAS_08082023"."K33P_TRANS_NF_ENTRADA" ( "DocNumPedTransf",
	 "DocNumTransf",
	 "DocEntryTransf",
	 "CardCode",
	 "DocDate",
	 "TaxDate",
	 "DocDueDate",
	 "U_K_TipoOrdem",
	 "BPL_IDAssignedToInvoice",
	 "SequenceCode",
	 "Origem",
	 "Destino",
	 "Tipo",
	 "Serial",
	 "U_ChaveAcesso" ) AS ((SELECT
	 DISTINCT IFNULL(WTR1."BaseRef",
	'') AS "DocNumPedTransf",
	 0 AS "DocNumTransf",
	 OWTR."DocEntry" AS "DocEntryTransf",
	 "@K33P_TRAN_PADC"."U_PNEntrada" AS "CardCode",
	 OINV."DocDate",
	 OINV."TaxDate",
	 OINV."DocDueDate",
	 "@K33P_TRAN_PADC"."U_FinalidSai" AS "U_K_TipoOrdem",
	 "@K33P_TRAN_PADC"."U_FilialEnt" AS "BPL_IDAssignedToInvoice",
	 -2 AS "SequenceCode",
	 WTQ1."FromWhsCod" AS "Origem",
	 OWHS."U_DepDePara" AS "Destino",
	 'Com Pedido' as "Tipo" ,
	 OINV."Serial" ,
	 PRO."KeyNfe" AS "U_ChaveAcesso" 
		FROM OWTR 
		INNER JOIN WTR1 ON OWTR."DocEntry" = WTR1."DocEntry" 
		INNER JOIN OBPL ON OWTR."BPLId" = OBPL."BPLId" 
		INNER JOIN NFN1 ON NFN1."BPLId" = OBPL."BPLId" 
		AND NFN1."Model" = 39 
		AND NFN1."Locked" = 'N' 
		INNER JOIN OWHS ON WTR1."WhsCode" = OWHS."WhsCode" 
		INNER JOIN WTQ1 ON WTR1."BaseEntry" = WTQ1."DocEntry" 
		AND WTR1."BaseLine" = WTQ1."LineNum" 
		AND WTR1."BaseType" = WTQ1."ObjType" 
		INNER JOIN OWTQ ON WTQ1."DocEntry" = OWTQ."DocEntry" 
		LEFT JOIN OINV ON OWTQ."DocNum" = OINV."U_NumPedTr" 
		AND OINV.CANCELED = 'N' 
		AND OINV."DocEntry" = ( SELECT
	 MAX(OINV_MAX."DocEntry") 
			FROM OINV OINV_MAX 
			WHERE OWTQ."DocNum" = OINV_MAX."U_NumPedTr" 
			AND OINV_MAX.CANCELED = 'N' ) 
		LEFT JOIN OPCH ON OWTQ."DocNum" = OPCH."U_NumPedTr" 
		AND OPCH.CANCELED = 'N' 
		AND OPCH."DocEntry" = ( SELECT
	 MAX(OPCH_MAX."DocEntry") 
			FROM OPCH OPCH_MAX 
			WHERE OWTQ."DocNum" = OPCH_MAX."U_NumPedTr" 
			AND OPCH_MAX.CANCELED = 'N' ) 
		LEFT JOIN ( SELECT
	 "DBInvOne"."Process"."DocEntry" ,
	 "DBInvOne"."Process"."DocType" ,
	 "SBO_TaxOne"."Entidade"."BusinessPlaceId" ,
	 "DBInvOne"."Process"."StatusId" ,
	 "DBInvOne"."ProcessHist"."Hist" ,
	 "DBInvOne"."Process"."KeyNfe" 
			FROM "DBInvOne"."Process" 
			INNER JOIN "SBO_TaxOne"."Entidade" ON "DBInvOne"."Process"."CompanyId" = "SBO_TaxOne"."Entidade".ID 
			AND "SBO_TaxOne"."Entidade"."CompanyDb" = CURRENT_SCHEMA 
			INNER JOIN "DBInvOne"."ProcessHist" ON "DBInvOne"."Process"."BatchId" = "DBInvOne"."ProcessHist" ."BatchId" 
			AND "DBInvOne"."ProcessHist"."Id" = ( SELECT
	 MAX(PRAHIS."Id") 
				FROM "DBInvOne"."ProcessHist" PRAHIS 
				WHERE "DBInvOne"."ProcessHist"."BatchId" = PRAHIS."BatchId" ) 
			WHERE "DBInvOne"."Process"."DocType" = 13 ) PRO ON PRO."DocEntry" = OINV."DocEntry" 
		AND PRO."DocType" = OINV."ObjType" 
		LEFT JOIN "@K33P_TRAN_PADC" ON OBPL."BPLId" = "@K33P_TRAN_PADC"."U_FilialSai" 
		AND OWHS."U_FilDePara" = "@K33P_TRAN_PADC"."U_FilialEnt" 
		WHERE IFNULL(OWHS."U_DepDePara",
	 '') <> '' 
		AND OWTQ.CANCELED = 'N' 
		AND PRO."StatusId" = 4) 
	UNION (SELECT
	 DISTINCT IFNULL(WTR1."BaseRef",
	'') AS "DocNumPedTransf",
	 IFNULL(OWTR."DocNum",
	 0) AS "DocNumTransf",
	 OWTR."DocEntry" AS "DocEntryTransf",
	 "@K33P_TRAN_PADC"."U_PNEntrada" AS "CardCode",
	 OINV."DocDate",
	 OINV."TaxDate",
	 OINV."DocDueDate",
	 "@K33P_TRAN_PADC"."U_FinalidSai" AS "U_K_TipoOrdem",
	 "@K33P_TRAN_PADC"."U_FilialEnt" AS "BPL_IDAssignedToInvoice",
	 -2 AS "SequenceCode",
	 WTR1."FromWhsCod" AS "Origem",
	 OWHS."U_DepDePara" AS "Destino",
	 'Sem Pedido' as "Tipo" ,
	 OINV."Serial" ,
	 PRO."KeyNfe" AS "U_ChaveAcesso" 
		FROM OWTR 
		INNER JOIN WTR1 ON OWTR."DocEntry" = WTR1."DocEntry" 
		INNER JOIN OBPL ON OWTR."BPLId" = OBPL."BPLId" 
		INNER JOIN NFN1 ON NFN1."BPLId" = OBPL."BPLId" 
		AND NFN1."Model" = 39 
		AND NFN1."Locked" = 'N' 
		INNER JOIN OWHS ON WTR1."WhsCode" = OWHS."WhsCode" 
		LEFT JOIN WTQ1 ON WTR1."BaseEntry" = WTQ1."DocEntry" 
		AND WTR1."BaseLine" = WTQ1."LineNum" 
		AND WTR1."BaseType" = WTQ1."ObjType" 
		LEFT JOIN OWTQ ON WTQ1."DocEntry" = OWTQ."DocEntry" 
		LEFT JOIN OINV ON OWTR."DocNum" = OINV."U_NumTransf" 
		AND OINV.CANCELED = 'N' 
		AND OINV."DocEntry" = ( SELECT
	 MAX(OINV_MAX."DocEntry") 
			FROM OINV OINV_MAX 
			WHERE OWTR."DocNum" = OINV_MAX."U_NumTransf" 
			AND OINV_MAX.CANCELED = 'N' ) 
		LEFT JOIN OPCH ON OWTR."DocNum" = OPCH."U_NumTransf" 
		AND OPCH.CANCELED = 'N' 
		AND OPCH."DocEntry" = ( SELECT
	 MAX(OPCH_MAX."DocEntry") 
			FROM OPCH OPCH_MAX 
			WHERE OWTR."DocNum" = OPCH_MAX."U_NumTransf" 
			AND OPCH_MAX.CANCELED = 'N' ) 
		LEFT JOIN ( SELECT
	 "DBInvOne"."Process"."DocEntry" ,
	 "DBInvOne"."Process"."DocType" ,
	 "SBO_TaxOne"."Entidade"."BusinessPlaceId" ,
	 "DBInvOne"."Process"."StatusId" ,
	 "DBInvOne"."ProcessHist"."Hist" ,
	 "DBInvOne"."Process"."KeyNfe" 
			FROM "DBInvOne"."Process" 
			INNER JOIN "SBO_TaxOne"."Entidade" ON "DBInvOne"."Process"."CompanyId" = "SBO_TaxOne"."Entidade".ID 
			AND "SBO_TaxOne"."Entidade"."CompanyDb" = CURRENT_SCHEMA 
			INNER JOIN "DBInvOne"."ProcessHist" ON "DBInvOne"."Process"."BatchId" = "DBInvOne"."ProcessHist" ."BatchId" 
			AND "DBInvOne"."ProcessHist"."Id" = ( SELECT
	 MAX(PRAHIS."Id") 
				FROM "DBInvOne"."ProcessHist" PRAHIS 
				WHERE "DBInvOne"."ProcessHist"."BatchId" = PRAHIS."BatchId" ) 
			WHERE "DBInvOne"."Process"."DocType" = 13 ) PRO ON PRO."DocEntry" = OINV."DocEntry" 
		AND PRO."DocType" = OINV."ObjType" 
		LEFT JOIN "@K33P_TRAN_PADC" ON OBPL."BPLId" = "@K33P_TRAN_PADC"."U_FilialSai" 
		AND OWHS."U_FilDePara" = "@K33P_TRAN_PADC"."U_FilialEnt" 
		WHERE IFNULL(OWHS."U_DepDePara",
	 '') <> '' 
		AND IFNULL(WTQ1."DocEntry",
	 0) = 0 
		AND OWTR.CANCELED = 'N' 
		AND PRO."StatusId" = 4)) WITH READ ONLY