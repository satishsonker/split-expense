import { useState } from 'react';
import { Outlet } from 'react-router-dom';
import { Box, AppBar, Toolbar, IconButton, Typography, Drawer } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import Header from './Header';
import LeftMenu from './LeftMenu';
import Footer from './Footer';

const Layout = () => {
    const [menuOpen, setMenuOpen] = useState(false);

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <Header onMenuClick={() => setMenuOpen(true)} />
            
            <Drawer
                anchor="left"
                open={menuOpen}
                onClose={() => setMenuOpen(false)}
            >
                <LeftMenu onClose={() => setMenuOpen(false)} />
            </Drawer>

            <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
                <Outlet />
            </Box>

            <Footer />
        </Box>
    );
};

export default Layout; 