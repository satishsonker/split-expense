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
import DeleteConfirmationDialog from '../components/DeleteConfirmationDialog';

const Contacts = () => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [contactList, setContactList] = useState([]);
    const [loading, setLoading] = useState(false);
    const [searchQuery, setSearchQuery] = useState('');
    const [openAddDialog, setOpenAddDialog] = useState(false);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [contactToDelete, setContactToDelete] = useState(null);
    const [deleteLoading, setDeleteLoading] = useState(false);

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

    const handleDeleteContact = async (contact) => {
        setContactToDelete(contact);
        setDeleteDialogOpen(true);
    };

    const handleConfirmDelete = async () => {
        try {
            setDeleteLoading(true);
            await apiService.delete(`${CONTACT_PATHS.DELETE}/${contactToDelete.userId}`);
            toast.success('Contact deleted successfully');
            fetchContacts();
        } catch (error) {
            console.error('Error deleting contact:', error);
            toast.error(error.response?.data?.message || 'Failed to delete contact');
        } finally {
            setDeleteLoading(false);
            setDeleteDialogOpen(false);
            setContactToDelete(null);
        }
    };

    const handleCloseDeleteDialog = () => {
        setDeleteDialogOpen(false);
        setContactToDelete(null);
    };

    const handleAddContact = async (contactData) => {
        try {
            setLoading(true);
            if (contactData.isExistingUser) {
                await apiService.post(`${CONTACT_PATHS.ADD_IN_CONTACT_LIST}${contactData.userId}`);
                toast.success('Contact added successfully');
            } else {
                await apiService.post(CONTACT_PATHS.CREATE, {
                    firstName: contactData.firstName,
                    lastName: contactData.lastName,
                    email: contactData.email,
                    phoneNumber: contactData.phoneNumber
                });
                toast.success('Contact created successfully');
            }
            setOpenAddDialog(false);
            setSearchQuery('');
            await fetchContacts();
        } catch (error) {
            console.error('Error adding contact:', error);
            toast.error(error.response?.data?.message || 'Failed to add contact');
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
                                            onClick={() => handleDeleteContact(contact)}
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

            <DeleteConfirmationDialog
                open={deleteDialogOpen}
                onClose={handleCloseDeleteDialog}
                onConfirm={handleConfirmDelete}
                loading={deleteLoading}
                type="contact"
                data={contactToDelete}
                title="Delete Contact"
                warningMessage="This action cannot be undone."
            />
        </Box>
    );
};

export default Contacts; 