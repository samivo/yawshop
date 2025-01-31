import { DataGrid, GridColDef, GridSortModel } from "@mui/x-data-grid";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Grid,
  TextField,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ApiEndpoint, ApiV1, Method } from "../../../Utilities/ApiFetch";
import { DateToString } from "../../../Utilities/DateToString";

interface Order {
  reference: string;
  totalAmount: number;
  clientId: number;
  client?: {
    firstName: string;
    lastName: string;
  };
  products: {
    unitPrice: number;
    units: number;
    vatPercentage: number;
    productCode: string;
    productName: string;
    eventCode: string | null;
  }[];
  transactionId: string | null;
  paymentStatus: number;
  paymentMethod: string | null;
  internalComment: string | null;
  createdAt: string;
  updatedAt: string;
  modifierName: string | null;
  hash: string | null;
  createdAt: string;
}

interface Filters {
  productCode?: string;
  customerName?: string;
  dateFrom?: string;
  dateTo?: string;
}

const OrderList: React.FC = () => {
  const navigate = useNavigate();

  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filters, setFilters] = useState<Filters>({});
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
      type: "number",
      valueGetter: (_value: Any, row: Any) => {
        return row.totalAmount / 100;
      },
    },
    {
      field: "paymentStatus",
      headerName: "Tila",
      width: 150,
      valueFormatter: (params) => {
        const statuses = {
          0: "Alustettu",
          1: "Uusi",
          2: "OK",
          3: "Epäonnistui",
          4: "Peruutettu",
          5: "Odottaa",
          6: "Viivästynyt",
        };
        return statuses[params.value] || params.value;
      },
    },
    {
      field: "createdAt",
      headerName: "Luontiaika",
      width: 200,
      type: "dateTime",
      valueGetter: (params) => {
        if (!params.value) return null;
        const date = new Date(params.value);
        return date;
      },
    },
  ];

  const fetchOrders = async () => {
    setLoading(true);
    setError(null);

    try {
      console.log(
        "Fetching orders with filters:",
        filters,
        "and sort:",
        sortModel,
      );
      const response = await ApiV1(ApiEndpoint.Orders, Method.GET, false, {
        ...filters,
        sortField: sortModel[0]?.field,
        sortOrder: sortModel[0]?.sort,
      });
      console.log("Raw API Response:", response);

      if (!response) {
        throw new Error("Ei vastausta palvelimelta");
      }

      const data = Array.isArray(response) ? response : [];
      console.log("Parsed API Response:", data);

      if (data.length > 0) {
        const mappedOrders = data.map((order: any) => {
          console.log("Original order data:", order);
          return {
            ...order,
            id: order.reference,
          };
        });
        console.log("Mapped orders:", mappedOrders);
        setOrders(mappedOrders);
      } else {
        console.log("No orders found");
        setOrders([]);
      }
    } catch (err: any) {
      console.error("Error fetching orders:", err);
      setError(err.message || "Tilausten hakemisessa tapahtui virhe");
      setOrders([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, [filters, sortModel]);

  const handleFilterChange =
    (field: keyof Filters) => (event: React.ChangeEvent<HTMLInputElement>) => {
      setFilters((prev) => ({
        ...prev,
        [field]: event.target.value,
      }));
    };

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

      <Grid container spacing={2} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={3}>
          <TextField
            fullWidth
            label="Tuotekoodi"
            variant="outlined"
            size="small"
            value={filters.productCode || ""}
            onChange={handleFilterChange("productCode")}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <TextField
            fullWidth
            label="Asiakkaan nimi"
            variant="outlined"
            size="small"
            value={filters.customerName || ""}
            onChange={handleFilterChange("customerName")}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <TextField
            fullWidth
            label="Alkaen"
            type="date"
            variant="outlined"
            size="small"
            InputLabelProps={{ shrink: true }}
            value={filters.dateFrom || ""}
            onChange={handleFilterChange("dateFrom")}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <TextField
            fullWidth
            label="Päättyen"
            type="date"
            variant="outlined"
            size="small"
            InputLabelProps={{ shrink: true }}
            value={filters.dateTo || ""}
            onChange={handleFilterChange("dateTo")}
          />
        </Grid>
      </Grid>

      <Box sx={{ height: 600, width: "100%" }}>
        <DataGrid
          rows={orders}
          columns={columns}
          getRowId={(row) => row.reference}
          loading={loading}
          sortModel={sortModel}
          onSortModelChange={handleSortModelChange}
          pageSize={10}
          rowsPerPageOptions={[10, 25, 50]}
          disableSelectionOnClick
          onRowClick={(params) => navigate(`/orders/${params.row.reference}`)}
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
            columnsPanelHideAllButton: "Piilota kaikki",
            columnsPanelShowAllButton: "Näytä kaikki",
            columnsPanelTextFieldLabel: "Etsi sarake",
            columnsPanelTextFieldPlaceholder: "Sarakkeen otsikko",
            filterPanelAddFilter: "Lisää suodatin",
            filterPanelDeleteIconLabel: "Poista",
            filterPanelLinkOperator: "Looginen operaattori",
            filterPanelOperators: "Operaattorit",
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

export default OrderList;
