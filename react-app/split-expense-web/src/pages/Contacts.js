import { useState, useEffect } from 'react';
import {
    Box,
    Typography,
    TextField,
    IconButton,
    Card,
    CardContent,
    Grid,
    Avatar,
    InputAdornment,
    Fab,
    useMediaQuery,
    useTheme
} from '@mui/material';
import {
    Search as SearchIcon,
    Add as AddIcon,
    Delete as DeleteIcon,
    Person as PersonIcon
} from '@mui/icons-material';
import { apiService } from '../utils/axios';
import { CONTACT_PATHS } from '../constants/apiPaths';
import { toast } from 'react-toastify';
import AddContactDialog from '../components/AddContactDialog';
import { debounce } from 'lodash';

const Contacts = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [contactList, setContactList] = useState([]);
    const [loading, setLoading] = useState(false);
    const [searchQuery, setSearchQuery] = useState('');
    const [openAddDialog, setOpenAddDialog] = useState(false);

    const fetchContacts = async () => {
        try {
            setLoading(true);
            const response = await apiService.get(CONTACT_PATHS.LIST, {
                pageNo: 1,
                pageSize: 500
            });
            setContactList(response.data || []);
        } catch (error) {
            console.error('Error fetching contacts:', error);
            toast.error('Failed to fetch contacts');
        } finally {
            setLoading(false);
        }
    };

    const searchContacts = async (query) => {
        try {
            setLoading(true);
            const response = await apiService.get(CONTACT_PATHS.SEARCH, {
                searchTerm: query
            });
            setContactList(response.data || []);
        } catch (error) {
            console.error('Error searching contacts:', error);
            toast.error('Failed to search contacts');
        } finally {
            setLoading(false);
        }
    };

    const debouncedSearch = debounce((query) => {
        if (query) {
            searchContacts(query);
        } else {
            fetchContacts();
        }
    }, 500);

    useEffect(() => {
        fetchContacts();
        return () => {
            debouncedSearch.cancel();
        };
    }, []);

    const handleSearch = (event) => {
        const query = event.target.value;
        setSearchQuery(query);
        debouncedSearch(query);
    };

    const handleDeleteContact = async (contactId) => {
        try {
            if (window.confirm('Are you sure you want to remove this contact?')) {
                await apiService.delete(`${CONTACT_PATHS.DELETE}/${contactId}`);
                toast.success('Contact removed successfully');
                fetchContacts();
            }
        } catch (error) {
            console.error('Error deleting contact:', error);
        }
    };

    const handleAddContact = async (contactData) => {
        try {
            setLoading(true);
            if (contactData.existingUser) {
                await apiService.post(CONTACT_PATHS.ADD, {
                    userId: contactData.userId
                });
            } else {
                await apiService.post(CONTACT_PATHS.CREATE, {
                    firstName: contactData.firstName,
                    lastName: contactData.lastName,
                    email: contactData.email,
                    phoneNumber: contactData.phoneNumber
                });
            }
            toast.success('Contact added successfully');
            setOpenAddDialog(false);
            fetchContacts();
        } catch (error) {
            console.error('Error adding contact:', error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box sx={{ p: { xs: 2, sm: 3 } }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h5" component="h1">
                    Contacts {loading && '(Loading...)'}
                </Typography>
                {!isMobile && (
                    <IconButton
                        color="primary"
                        onClick={() => setOpenAddDialog(true)}
                        sx={{ bgcolor: 'primary.light', '&:hover': { bgcolor: 'primary.main' } }}
                    >
                        <AddIcon />
                    </IconButton>
                )}
            </Box>

            <TextField
                fullWidth
                variant="outlined"
                placeholder="Search by name, email or phone..."
                value={searchQuery}
                onChange={handleSearch}
                sx={{ mb: 3 }}
                InputProps={{
                    startAdornment: (
                        <InputAdornment position="start">
                            <SearchIcon />
                        </InputAdornment>
                    ),
                }}
            />

            <Grid container spacing={2}>
                {contactList.length === 0 && !loading ? (
                    <Grid item xs={12}>
                        <Typography variant="body1" color="textSecondary" align="center">
                            {searchQuery ? 'No contacts found' : 'No contacts yet'}
                        </Typography>
                    </Grid>
                ) : (
                    contactList.map(contact => (
                        <Grid item xs={12} sm={6} md={4} key={contact.id}>
                            <Card>
                                <CardContent>
                                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                            <Avatar 
                                                src={contact.contactUser.profilePicture}
                                                sx={{ mr: 2 }}
                                            >
                                                {contact.contactUser.firstName[0]}
                                            </Avatar>
                                            <Box>
                                                <Typography variant="subtitle1">
                                                    {`${contact.contactUser.firstName} ${contact.contactUser.lastName || ''}`}
                                                </Typography>
                                                <Typography variant="body2" color="textSecondary">
                                                    {contact.contactUser.email}
                                                </Typography>
                                                {contact.contactUser.phoneNumber && (
                                                    <Typography variant="body2" color="textSecondary">
                                                        {contact.contactUser.phoneNumber}
                                                    </Typography>
                                                )}
                                            </Box>
                                        </Box>
                                        <IconButton
                                            size="small"
                                            color="error"
                                            onClick={() => handleDeleteContact(contact.id)}
                                            disabled={loading}
                                        >
                                            <DeleteIcon />
                                        </IconButton>
                                    </Box>
                                </CardContent>
                            </Card>
                        </Grid>
                    ))
                )}
            </Grid>

            {isMobile && (
                <Fab
                    color="primary"
                    sx={{ position: 'fixed', bottom: 16, right: 16 }}
                    onClick={() => setOpenAddDialog(true)}
                >
                    <AddIcon />
                </Fab>
            )}

            <AddContactDialog
                open={openAddDialog}
                onClose={() => setOpenAddDialog(false)}
                onSubmit={handleAddContact}
            />
        </Box>
    );
};

export default Contacts; 