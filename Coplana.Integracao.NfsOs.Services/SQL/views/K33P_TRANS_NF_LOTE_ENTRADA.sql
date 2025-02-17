CREATE VIEW "COPLANA_PRD"."K33P_TRANS_NF_LOTE_ENTRADA" ( "DocEntry",
	 "LineNum",
	 "ItemCode",
	 "BaseRef",
	 "BaseType",
	 "Quantity",
	 "BatchNumber",
	 "SystemSerialNumber" ) AS ((SELECT
	 WTR1."DocEntry" ,
	 WTR1."LineNum",
	 WTR1."ItemCode",
	 WTR1."BaseRef" ,
	 WTR1."ObjType" "BaseType" ,
	 IBT1."Quantity",
	 IBT1."BatchNum" as "BatchNumber",
	 (SELECT
	 "SysNumber" 
			FROM OBTN 
			WHERE OBTN."ItemCode" = WTR1."ItemCode" 
			AND "DistNumber" = IBT1."BatchNum" ) as "SystemSerialNumber" 
		FROM WTR1 
		INNER JOIN OITM ON WTR1."ItemCode" = OITM."ItemCode" 
		INNER JOIN IBT1 ON WTR1 ."ItemCode" = IBT1."ItemCode" 
		AND WTR1."WhsCode" = IBT1."WhsCode" 
		AND WTR1."ObjType" = IBT1."BaseType" 
		AND WTR1."DocEntry" = IBT1."BaseEntry" 
		AND WTR1."LineNum" = IBT1."BaseLinNum" 
		WHERE WTR1."ObjType" = 67 
		AND IBT1."Direction" <> 2 
		AND IBT1."WhsCode" LIKE '10-26%') 
	UNION ALL (SELECT
	 INV1."DocEntry" ,
	 INV1."LineNum",
	 INV1."ItemCode",
	 INV1."BaseRef" ,
	 INV1."ObjType" "BaseType" ,
	 IBT1."Quantity",
	 IBT1."BatchNum" as "BatchNumber",
	 (SELECT
	 "SysNumber" 
			FROM OBTN 
			WHERE OBTN."ItemCode" = INV1."ItemCode" 
			AND "DistNumber" = IBT1."BatchNum" ) as "SystemSerialNumber" 
		FROM INV1 
		INNER JOIN OITM ON INV1."ItemCode" = OITM."ItemCode" 
		INNER JOIN OINV ON INV1."DocEntry" = OINV."DocEntry" 
		INNER JOIN IBT1 ON INV1 ."ItemCode" = IBT1."ItemCode" 
		AND INV1."WhsCode" = IBT1."WhsCode" 
		AND INV1."ObjType" = IBT1."BaseType" 
		AND INV1."DocEntry" = IBT1."BaseEntry" 
		AND INV1."LineNum" = IBT1."BaseLinNum" 
		WHERE IFNULL(OINV."U_NumTransf",
	 '') = '' 
		AND IBT1."Direction" <> 2 
		AND INV1."ObjType" = 13 
		AND IFNULL(OINV."U_NumPedTr",
	 '') = '') 
	ORDER BY 1,
	2) WITH READ ONLY