CREATE VIEW "COPLANA_PRD"."K33P_TRANS_NF_ITEM_SAIDA" ( "DocNum",
	 "INV12 Incoterms",
	 "Quantity",
	 "Price",
	 "WarehouseCode",
	 "Usage",
	 "Origem",
	 "Destino",
	 "Tipo",
	 "ItemCode",
	 "BinAbsEntry",
	 "U_K_TipoOrdem" ) AS SELECT
	 DISTINCT OWTR."DocNum" ,
	 "@K33P_TRAN_PADC"."U_Incoterms" AS "INV12 Incoterms",
	 SUM(WTR1."Quantity") AS "Quantity",
	 SUM(WTR1."StockPrice") AS "Price",
	 WTR1."WhsCode" AS "WarehouseCode",
	 CASE WHEN WTR1."ItemCode" LIKE 'EM%' 
THEN 5 
ELSE "@K33P_TRAN_PADC"."U_UtilizSai" 
END AS "Usage",
	 WTR1."FromWhsCod" AS "Origem",
	 WTR1."WhsCode" AS "Destino",
	 'Sem Pedido' AS "Tipo",
	 WTR1."ItemCode",
	 OWHS."DftBinAbs" AS "BinAbsEntry",
	 CASE WHEN WTR1."ItemCode" LIKE 'EM%' 
THEN 'D' 
ELSE 'E' 
END AS "U_K_TipoOrdem" 
FROM OWTR 
INNER JOIN WTR1 ON OWTR."DocEntry" = WTR1."DocEntry" 
INNER JOIN OBPL ON OWTR."BPLId" = OBPL."BPLId" 
INNER JOIN OWHS ON WTR1."WhsCode" = OWHS."WhsCode" 
LEFT JOIN WTQ1 ON WTR1."BaseEntry" = WTQ1."DocEntry" 
AND WTR1."BaseLine" = WTQ1."LineNum" 
AND WTR1."BaseType" = WTQ1."ObjType" 
LEFT JOIN OWTQ ON WTQ1."DocEntry" = OWTQ."DocEntry" 
LEFT JOIN OINV ON OWTR."DocNum" = OINV."U_NumPedTr" 
AND OINV.CANCELED = 'N' 
AND OINV."DocEntry" = ( SELECT
	 MAX(OINV_MAX."DocEntry") 
	FROM OINV OINV_MAX 
	WHERE OWTR."DocNum" = OINV_MAX."U_NumPedTr" 
	AND OINV_MAX.CANCELED = 'N' ) 
LEFT JOIN OPCH ON OWTR."DocNum" = OPCH."U_NumPedTr" 
AND OPCH.CANCELED = 'N' 
AND OPCH."DocEntry" = ( SELECT
	 MAX(OPCH_MAX."DocEntry") 
	FROM OPCH OPCH_MAX 
	WHERE OWTR."DocNum" = OPCH_MAX."U_NumPedTr" 
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
AND WTR1."Quantity" > 0 
AND IFNULL(OWTR."U_ImportNFS",
	 'N') = 'N' 
GROUP BY OWTR."DocNum" ,
	 "@K33P_TRAN_PADC"."U_Incoterms" ,
	 WTQ1."FromWhsCod" ,
	 "@K33P_TRAN_PADC"."U_UtilizSai",
	 WTR1."FromWhsCod" ,
	 WTR1."WhsCode" ,
	 WTR1."ItemCode",
	 OWHS."U_DepDePara",
	 OWHS."DftBinAbs" WITH READ ONLY