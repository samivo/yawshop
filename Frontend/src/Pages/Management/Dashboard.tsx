import Typography from '@mui/material/Typography';
import { createTheme } from '@mui/material/styles';
import DashboardIcon from '@mui/icons-material/Dashboard';
import ShoppingCartIcon from '@mui/icons-material/ShoppingCart';
import BarChartIcon from '@mui/icons-material/BarChart';
import DescriptionIcon from '@mui/icons-material/Description';
import LayersIcon from '@mui/icons-material/Layers';
import { AppProvider, type Navigation } from '@toolpad/core/AppProvider';
import { DashboardLayout } from '@toolpad/core/DashboardLayout';
import { useDemoRouter } from '@toolpad/core/internal';
import { Stack } from '@mui/material';
import CurrencyExchangeIcon from '@mui/icons-material/CurrencyExchange';
import EventIcon from '@mui/icons-material/Event';
import CardGiftcardIcon from '@mui/icons-material/CardGiftcard';
import DiscountIcon from '@mui/icons-material/Discount';
import ProductList from './Products/ProductList';
import { EventList } from './Events/EventList';
import CheckoutList from './Checkouts/CheckoutList';

const NAVIGATION: Navigation = [
  {
    kind: 'header',
    title: 'Hallinta',
  },
  {
    segment: 'dashboard',
    title: 'Etusivu',
    icon: <DashboardIcon />,
  },
  {
    segment: 'checkouts',
    title: 'Ostot',
    icon: <CurrencyExchangeIcon />,
  },
  {
    segment: 'products',
    title: 'Tuotteet',
    icon: <ShoppingCartIcon />,
    
  },
  {
    segment: 'events',
    title: 'Tapahtumat',
    icon: <EventIcon />,
    
  },
  {
    segment: 'giftcards',
    title: 'Lahjakortit',
    icon: <CardGiftcardIcon />,
  },
  {
    segment: 'discounts',
    title: 'Alennuskoodit',
    icon: <DiscountIcon />,
  },
  
  {
    kind: 'divider',
  },
  {
    kind: 'header',
    title: 'Analytiikka',
  },
  {
    segment: 'reports',
    title: 'Raportit',
    icon: <BarChartIcon />,
    children: [
      {
        segment: 'sales',
        title: 'Sales',
        icon: <DescriptionIcon />,
      },
      {
        segment: 'traffic',
        title: 'Traffic',
        icon: <DescriptionIcon />,
      },
    ],
  },
  {
    segment: 'integrations',
    title: 'Integrations',
    icon: <LayersIcon />,
  },
];

const demoTheme = createTheme({
  cssVariables: {
    colorSchemeSelector: 'data-toolpad-color-scheme',
  },
  colorSchemes: { light: true, dark: true },
  breakpoints: {
    values: {
      xs: 0,
      sm: 600,
      md: 600,
      lg: 1200,
      xl: 1536,
    },
  },
});

function DashboardPage({ pathname }: { pathname: string }) {

  switch (pathname) {
    case "/products":
      return (<ProductList />);
    case "/events":
      return (<EventList/>);
    case "/checkouts":
      return (<CheckoutList />);
    
  }

}

function CustomAppTitle() {
    return (
      <Stack direction="row" alignItems="center" spacing={2}>
        <Typography variant="h6">KLU kauppa</Typography>
      </Stack>
    );
  }


export default function DashboardLayoutBasic() {

  const router = useDemoRouter('/dashboard');

  return (
    // preview-start
    <AppProvider
      navigation={NAVIGATION}
      router={router}
      theme={demoTheme}
    >
      <DashboardLayout
      slots={{
        appTitle: CustomAppTitle,
      }}
      >
        <DashboardPage pathname={router.pathname} />
      </DashboardLayout>
    </AppProvider>
    // preview-end
  );

}