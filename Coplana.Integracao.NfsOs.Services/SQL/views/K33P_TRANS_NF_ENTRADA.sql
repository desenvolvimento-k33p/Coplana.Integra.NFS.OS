CREATE VIEW "COPLANA_PRD"."K33P_TRANS_NF_ENTRADA" ( "DocNumPedTransf",
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
	 "SequenceSerial",
	 "U_ChaveAcesso",
	 "SeriesString",
	 "GerarEsboco" ) AS (((SELECT
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
	 'Com Pedido' AS "Tipo" ,
	 OINV."Serial" AS "SequenceSerial",
	 PRO."KeyNfe" AS "U_ChaveAcesso" ,
	 OINV."SeriesStr" AS "SeriesString",
	 'AA' AS "GerarEsboco" -- N = Não e E = Sim
 
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
			AND PRO."StatusId" = 4 
			AND IFNULL(OWTR."U_ImportNFE",
	 'N') = 'N') 
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
	 'Sem Pedido' AS "Tipo" ,
	 OINV."Serial" AS "SequenceSerial",
	 PRO."KeyNfe" AS "U_ChaveAcesso" ,
	 OINV."SeriesStr" AS "SeriesString",
	 'N' AS "GerarEsboco" -- N = Não e E = Sim -- 0 = Não e 1 = Sim
 
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
			AND PRO."StatusId" = 4 
			AND IFNULL(OWTR."U_ImportNFE",
	 'N') = 'N')) 
	UNION (SELECT
	 DISTINCT IFNULL(TO_NVARCHAR( OINV."DocEntry"),
	 '') AS "DocNumPedTransf",
	 OINV."DocNum" AS "DocNumTransf",
	 OINV."DocEntry" AS "DocEntryTransf",
	 F."DflVendor" AS "CardCode",
	 OINV."DocDate",
	 OINV."TaxDate",
	 OINV."DocDueDate",
	 'E' AS "U_K_TipoOrdem",
	 OBPL."BPLId" AS "BPL_IDAssignedToInvoice",
	 -2 AS "SequenceCode",
	 MAX(INV1."WhsCode") AS "Origem",
	 MAX(OBPL."DflWhs") AS "Destino",
	 'Transf.' AS "Tipo" ,
	 OINV."Serial" AS "SequenceSerial",
	 PRO."KeyNfe" AS "U_ChaveAcesso" ,
	 OINV."SeriesStr" AS "SeriesString",
	 CASE WHEN OUSG."U_K33P_EscrituraNF" = 'E' 
		THEN 'Y' 
		ELSE 'N' 
		END AS "GerarEsboco" -- N = Não e E = Sim
 
		FROM OINV 
		INNER JOIN INV1 ON OINV."DocEntry" = INV1."DocEntry" --WTR1 ON OWTR."DocEntry" = WTR1."DocEntry" 
 
		INNER JOIN OBPL ON OINV."CardCode" = OBPL."DflCust" 
		INNER JOIN OBPL F ON OINV."BPLId" = F."BPLId" 
		INNER JOIN NFN1 ON NFN1."BPLId" = OBPL."BPLId" 
		AND NFN1."Model" = 39 
		AND NFN1."Locked" = 'N' 
		INNER JOIN OUSG ON OUSG."ID" = INV1."Usage" 
		INNER JOIN OWHS ON INV1."WhsCode" = OWHS."WhsCode" --INNER JOIN WTQ1 ON WTR1."BaseEntry" = WTQ1."DocEntry" AND WTR1."BaseLine" = WTQ1."LineNum" AND WTR1."BaseType" = WTQ1."ObjType" 
--INNER JOIN OWTQ ON WTQ1."DocEntry" = OWTQ."DocEntry" 
--	LEFT JOIN OINV ON OWTQ."DocNum" = OINV."U_NumPedTr" AND OINV.CANCELED = 'N' 
--		AND OINV."DocEntry" = ( SELECT MAX(OINV_MAX."DocEntry") FROM OINV OINV_MAX 
--								WHERE OWTQ."DocNum" = OINV_MAX."U_NumPedTr" AND OINV_MAX.CANCELED = 'N' ) 
--	LEFT JOIN OPCH ON OWTQ."DocNum" = OPCH."U_NumPedTr"  AND OPCH.CANCELED = 'N' 
--		AND OPCH."DocEntry" = ( SELECT MAX(OPCH_MAX."DocEntry") FROM OPCH OPCH_MAX 
--								WHERE OWTQ."DocNum" = OPCH_MAX."U_NumPedTr" AND OPCH_MAX.CANCELED = 'N' ) 
 
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
		AND PRO."DocType" = OINV."ObjType" --LEFT JOIN "@K33P_TRAN_PADC" ON OBPL."BPLId" = "@K33P_TRAN_PADC"."U_FilialSai" AND OWHS."U_FilDePara" = "@K33P_TRAN_PADC"."U_FilialEnt" 
 
		WHERE OINV.CANCELED = 'N' 
		AND OINV."DocDate" >= '2024-06-06' 
		AND PRO."StatusId" = 4 
		AND IFNULL(OINV."U_NumTransf",
	 '') = '' 
		AND IFNULL(OINV."U_NumPedTr",
	 '') = '' --AND OINV."Serial" = 221231
--AND OINV."DocNum" IN (1857)
 
		AND NOT EXISTS ( SELECT
	 OPCH."DocEntry" 
			FROM OPCH 
			WHERE OPCH."Serial" = OINV."Serial" 
			AND OINV."SeriesStr" = OPCH."SeriesStr" 
			AND OINV."Model" = OPCH."Model" 
			UNION SELECT
	 ODRF."DocEntry" 
			FROM ODRF 
			WHERE ODRF."Serial" = OINV."Serial" 
			AND OINV."SeriesStr" = ODRF."SeriesStr" 
			AND OINV."Model" = ODRF."Model" ) 
		GROUP BY OINV."DocEntry",
	 OINV."DocNum" ,
	 OINV."DocEntry" ,
	 F."DflVendor" ,
	 OINV."DocDate",
	 OINV."TaxDate",
	 OINV."DocDueDate",
	 OBPL."BPLId" ,
	 OINV."Serial" ,
	 PRO."KeyNfe" ,
	 OINV."SeriesStr",
	 OUSG."U_K33P_EscrituraNF")) WITH READ ONLY