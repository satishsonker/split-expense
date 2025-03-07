import React from 'react';
import { Box } from '@mui/material';
import {
    CurrencyRupee as RupeeIcon,
    CurrencyPound as PoundIcon,
    CurrencyYen as YenIcon,
    Euro as EuroIcon,
    AttachMoney as DollarIcon
} from '@mui/icons-material';

const CurrencyIcon = ({ amount, sx = {}, ...props }) => {
    // Get currency from localStorage
    const user = JSON.parse(localStorage.getItem('user'));
    const currency = user?.currencyCode || 'USD';

    const formatAmount = (value) => {
        if (value === undefined || value === null) return null;
        return Number(value).toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    };

    const getCurrencyIcon = () => {
        const formattedAmount = formatAmount(amount);
        const iconProps = { sx, ...props };

        switch (currency) {
            case 'INR':
                return (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <RupeeIcon {...iconProps} />
                        {formattedAmount}
                    </Box>
                );
            case 'GBP':
                return (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <PoundIcon {...iconProps} />
                        {formattedAmount}
                    </Box>
                );
            case 'JPY':
                return (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <YenIcon {...iconProps} />
                        {formattedAmount}
                    </Box>
                );
            case 'EUR':
                return (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <EuroIcon {...iconProps} />
                        {formattedAmount}
                    </Box>
                );
            case 'USD':
            default:
                return (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <DollarIcon {...iconProps} />
                        {formattedAmount}
                    </Box>
                );
        }
    };

    return getCurrencyIcon();
};

export default React.memo(CurrencyIcon); 