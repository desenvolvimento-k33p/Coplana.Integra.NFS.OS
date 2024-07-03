CREATE VIEW "COPLANA_PRD"."K33P_TRANS_NF_ITEM_ENTRADA" ( "DocNum",
	 "INV12 Incoterms",
	 "Quantity",
	 "Price",
	 "WarehouseCode",
	 "Usage",
	 "Origem",
	 "Destino",
	 "Tipo",
	 "ItemCode",
	 "BaseType",
	 "BaseEntry",
	 "BaseLine",
	 "BinAbsEntry" ) AS ((SELECT
	 DISTINCT OWTR."DocNum" ,
	 "@K33P_TRAN_PADC"."U_Incoterms" AS "INV12 Incoterms",
	 SUM(WTR1."Quantity") AS "Quantity",
	 SUM(WTR1."StockPrice") AS "Price",
	 OWHS."U_DepDePara" AS "WarehouseCode",
	 "@K33P_TRAN_PADC"."U_UtilizEnt" AS "Usage",
	 WTR1."FromWhsCod" AS "Origem",
	 OWHS."U_DepDePara" AS "Destino",
	 'Sem Pedido' AS "Tipo",
	 WTR1."ItemCode",
	 -1 "BaseType" ,
	 NULL "BaseEntry" ,
	 NULL "BaseLine" ,
	 OWHS_DEP."DftBinAbs" AS "BinAbsEntry" 
		FROM OWTR 
		INNER JOIN WTR1 ON OWTR."DocEntry" = WTR1."DocEntry" 
		INNER JOIN OBPL ON OWTR."BPLId" = OBPL."BPLId" 
		INNER JOIN OWHS ON WTR1."WhsCode" = OWHS."WhsCode" 
		LEFT JOIN OWHS OWHS_DEP ON OWHS."U_DepDePara" = OWHS_DEP."WhsCode" 
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
	 "DBInvOne"."ProcessHist"."Hist" 
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
		AND OWTR.CANCELED = 'N' 
		AND PRO."StatusId" = 4 
		AND IFNULL(OWTR."U_ImportNFE",
	 'N') = 'N' 
		GROUP BY OWTR."DocNum" ,
	 "@K33P_TRAN_PADC"."U_Incoterms" ,
	 WTR1."ItemCode",
	 "@K33P_TRAN_PADC"."U_UtilizEnt",
	 WTR1."FromWhsCod" ,
	 WTR1."WhsCode" ,
	 OWHS."U_DepDePara",
	 OWHS_DEP."DftBinAbs") 
	UNION (SELECT
	 DISTINCT OINV."DocNum" ,
	 INV12."Incoterms" "INV12 Incoterms",
	 SUM(INV1."Quantity") AS "Quantity",
	 MAX(INV1."PriceBefDi") AS "Price",
	 INV1."WhsCode" AS "WarehouseCode",
	 INV1."Usage" AS "Usage",
	 INV1."WhsCode" AS "Origem",
	 OBPL."DflWhs" AS "Destino",
	 'Transf.' AS "Tipo",
	 INV1."ItemCode",
	 CASE WHEN IFNULL(INV1."U_xPed",
	 '') <> '' 
		THEN 22 
		ELSE NULL 
		END "BaseType" ,
	 OPOR_DOC."DocEntry" AS "U_xPed" ,
	 OPOR_DOC."LineNum" AS "U_nItem",
	 OWHS_DEP."DftBinAbs" 
		FROM OINV 
		INNER JOIN INV1 ON OINV."DocEntry" = INV1."DocEntry" --WTR1 ON OWTR."DocEntry" = WTR1."DocEntry" 

		INNER JOIN INV12 ON OINV."DocEntry" = INV12."DocEntry" 
		INNER JOIN OBPL ON OINV."CardCode" = OBPL."DflCust" 
		INNER JOIN OBPL F ON OINV."BPLId" = F."BPLId" 
		INNER JOIN NFN1 ON NFN1."BPLId" = OBPL."BPLId" 
		AND NFN1."Model" = 39 
		AND NFN1."Locked" = 'N' 
		LEFT JOIN ( SELECT
	 OPOR."DocNum" ,
	 OPOR."DocEntry" ,
	 POR1."LineNum" 
			FROM OPOR 
			INNER JOIN POR1 ON OPOR."DocEntry" = POR1."DocEntry" 
			WHERE OPOR.CANCELED = 'N' ) OPOR_DOC ON INV1."U_xPed" = OPOR_DOC."DocNum" 
		AND (INV1."U_nItem" -1) = OPOR_DOC."LineNum" 
		INNER JOIN OWHS ON INV1."WhsCode" = OWHS."WhsCode" 
		LEFT JOIN OWHS OWHS_DEP ON OBPL."DflWhs" = OWHS_DEP."WhsCode" 
		LEFT JOIN ( SELECT
	 "DBInvOne"."Process"."DocEntry" ,
	 "DBInvOne"."Process"."DocType" ,
	 "SBO_TaxOne"."Entidade"."BusinessPlaceId" ,
	 "DBInvOne"."Process"."StatusId" ,
	 "DBInvOne"."ProcessHist"."Hist" 
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
		WHERE OINV.CANCELED = 'N' 
		AND PRO."StatusId" = 4 
		AND OINV."DocDate" >= '2024-06-06' 
		AND NOT EXISTS ( SELECT
	 * 
			FROM OPCH 
			WHERE OPCH."Serial" = OINV."Serial" 
			AND OINV."SeriesStr" = OPCH."SeriesStr" 
			AND OINV."Model" = OPCH."Model" 
			AND OPCH.CANCELED = 'N') 
		GROUP BY OINV."DocNum" ,
	 INV12."Incoterms",
	 OWHS."U_DepDePara",
	 INV1."Usage",
	 INV1."WhsCode",
	 OBPL."DflWhs",
	 INV1."U_xPed" ,
	 INV1."U_nItem" ,
	 'Transf.' ,
	 INV1."ItemCode",
	 OPOR_DOC."DocEntry",
	 OPOR_DOC."LineNum",
	 OWHS_DEP."DftBinAbs")) WITH READ ONLY