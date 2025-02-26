import { DataGrid, GridColDef, GridSortModel } from "@mui/x-data-grid";
import {
  Alert,
  Box,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { ApiEndpoint, ApiV1, Method } from "../../../Utilities/ApiFetch";
import { CheckoutModel } from "../../../Models/CheckoutModel";



interface Filters {
  productCode?: string;
  customerName?: string;
  dateFrom?: string;
  dateTo?: string;
}

const CheckoutList: React.FC = () => {

  const [checkouts, SetCheckouts] = useState<CheckoutModel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filters, _setFilters] = useState<Filters>({});
  const [sortModel, setSortModel] = useState<GridSortModel>([
    { field: "id", sort: "asc" },
  ]);

  const columns: GridColDef[] = [
    {
      field: "reference",
      headerName: "Viite",
      width: 250,
    },
    {
      field: "totalAmount",
      headerName: "Summa",
      width: 120,
      type: "string",
      align:'center',
        valueGetter: (_value, row) => {
        return row.totalAmount / 100+" €";
      },
    },
    {
      field: "paymentStatus",
      headerName: "Tila",
      width: 150,
      valueFormatter: (value) => {
        const statuses = {
          0: "Alustettu",
          1: "Uusi",
          2: "OK",
          3: "Epäonnistui",
          4: "Peruutettu",
          5: "Odottaa",
          6: "Viivästynyt",
        };
          return statuses[value] || value;
      },
    },
    {
      field: "createdAt",
      headerName: "Luontiaika",
      width: 200,
      type: "dateTime",
        valueGetter: (value) => {
        return new Date(value);
        
      },
    },
  ];

  const fetchOrders = async () => {
    setLoading(true);
    setError(null);

        try {

            const response: CheckoutModel[] = await ApiV1(ApiEndpoint.Checkout, Method.GET, false);

            if (!response) {
                throw new Error("Ei vastausta palvelimelta");
            }

            const data = Array.isArray(response) ? response : [];

            SetCheckouts(data);

        } catch (err: any) {
            console.error("Error fetching orders:", err);
            setError(err.message || "Tilausten hakemisessa tapahtui virhe");
            SetCheckouts([]);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchOrders();
    }, [filters, sortModel]);

  // const handleFilterChange =
  //   (field: keyof Filters) => (event: React.ChangeEvent<HTMLInputElement>) => {
  //     setFilters((prev) => ({
  //       ...prev,
  //       [field]: event.target.value,
  //     }));
  //   };

  const handleSortModelChange = (newModel: GridSortModel) => {
    setSortModel(newModel);
  };

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Myydyt tuotteet
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      

      <Box sx={{ height: 600, width: "100%" }}>
        <DataGrid
          rows={checkouts}
          columns={columns}
          getRowId={(row) => row.reference}
          loading={loading}
          sortModel={sortModel}
          onSortModelChange={handleSortModelChange}
          sx={{
            "& .MuiDataGrid-row": {
              cursor: "pointer",
              "&:hover": { backgroundColor: "rgba(0, 0, 0, 0.04)" },
            },
          }}
          localeText={{
            noRowsLabel: "Ei tilauksia",
            footerRowSelected: (count) =>
              `${count.toLocaleString()} rivi${count !== 1 ? "ä" : ""} valittu`,
            columnMenuSortAsc: "Lajittele nousevasti",
            columnMenuSortDesc: "Lajittele laskevasti",
            columnMenuFilter: "Suodata",
            columnMenuHideColumn: "Piilota sarake",
            columnMenuManageColumns: "Hallitse sarakkeita",
            filterPanelAddFilter: "Lisää suodatin",
            filterPanelDeleteIconLabel: "Poista",
            filterPanelOperatorAnd: "Ja",
            filterPanelOperatorOr: "Tai",
            filterPanelColumns: "Sarakkeet",
            filterPanelInputLabel: "Arvo",
            filterPanelInputPlaceholder: "Suodatusarvo",
            toolbarColumns: "Sarakkeet",
            toolbarFilters: "Suodattimet",
            toolbarDensity: "Tiheys",
            toolbarDensityCompact: "Tiivis",
            toolbarDensityStandard: "Normaali",
            toolbarDensityComfortable: "Leveä",
          }}
        />
      </Box>
    </Box>
  );
};

export default CheckoutList;