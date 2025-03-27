import React, { useState, useEffect, useCallback } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    TextField,
    List,
    ListItem,
    ListItemAvatar,
    ListItemText,
    Avatar,
    IconButton,
    Typography,
    Box,
    CircularProgress,
    InputAdornment,
    useTheme,
    useMediaQuery,
    Tabs,
    Tab,
    Divider
} from '@mui/material';
import {
    Search as SearchIcon,
    Add as AddIcon,
    Close as CloseIcon,
    ContactMail as ContactIcon
} from '@mui/icons-material';
import { toast } from 'react-toastify';
import { apiService } from '../utils/axios';
import { GROUP_PATHS,CONTACT_PATHS } from '../constants/apiPaths';
import { getImageUrl } from '../utils/imageUtils';
import debounce from 'lodash/debounce';

// Move debounced search outside component to prevent recreation
const createDebouncedSearch = (callback) => 
    debounce(async (term, signal) => {
        if (!term) {
            callback({ results: [], error: null });
            return;
        }

        try {
            const response = await apiService.get(
                `${CONTACT_PATHS.SEARCH_USER}?searchTerm=${term}`,
                { signal }
            );
            callback({ results: response, error: null });
        } catch (error) {
            if (error.name === 'AbortError') return;
            callback({ results: [], error });
        }
    }, 300); // Reduced from 500ms to 300ms for better responsiveness

const AddGroupMemberDialog = ({ open, onClose, groupId, existingMembers = [] }) => {
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
    const [activeTab, setActiveTab] = useState(0);
    const [searchTerm, setSearchTerm] = useState('');
    const [searching, setSearching] = useState(false);
    const [searchResults, setSearchResults] = useState([]);
    const [selectedUsers, setSelectedUsers] = useState([]);
    const [adding, setAdding] = useState(false);
    const [contacts, setContacts] = useState([]);
    const [loadingContacts, setLoadingContacts] = useState(false);
    const [debouncedSearchFn] = useState(() => createDebouncedSearch(
        ({ results, error }) => {
            setSearching(false);
            if (error) {
                console.error('Error searching users:', error);
                toast.error('Failed to search users');
                setSearchResults([]);
                return;
            }

            // Filter out existing members and already selected users
            const filteredResults = results.filter(user => 
                !existingMembers.some(member => member.addedUser.id === user.id) &&
                !selectedUsers.some(selected => selected.id === user.id)
            );
            setSearchResults(filteredResults);
        }
    ));

    // Cleanup debounced function on unmount
    useEffect(() => {
        return () => {
            debouncedSearchFn.cancel();
        };
    }, [debouncedSearchFn]);

    useEffect(() => {
        if (open) {
            fetchContacts();
        }
    }, [open]);

    const fetchContacts = async () => {
        try {
            setLoadingContacts(true);
            const response = await apiService.get(GROUP_PATHS.MEMBERS);
            // Filter out existing members and already selected users
            const filteredContacts = response?.data?.filter(contact => 
                !existingMembers.some(member => member.addedUser.id === contact.contactUser.id) &&
                !selectedUsers.some(selected => selected.id === contact.contactUser.id)
            );
            setContacts(filteredContacts);
        } catch (error) {
            console.error('Error fetching contacts:', error);
            toast.error('Failed to load contacts');
        } finally {
            setLoadingContacts(false);
        }
    };

    const handleSearchChange = useCallback((event) => {
        const value = event.target.value;
        setSearchTerm(value);
        if (value) {
            setSearching(true);
        }
        // Create abort controller for each search
        const controller = new AbortController();
        debouncedSearchFn(value, controller.signal);
        
        return () => {
            controller.abort();
        };
    }, [debouncedSearchFn]);

    const handleSelectUser = (user) => {
        setSelectedUsers(prev => [...prev, user]);
        setSearchResults(prev => prev.filter(u => u.id !== user.id));
        setContacts(prev => prev.filter(c => c.contactUser.id !== user.id));
        setSearchTerm('');
    };

    const handleSelectContact = (contact) => {
        setSelectedUsers(prev => [...prev, contact.contactUser]);
        setContacts(prev => prev.filter(c => c.id !== contact.id));
    };

    const handleRemoveUser = (userId) => {
        const removedUser = selectedUsers.find(u => u.id === userId);
        setSelectedUsers(prev => prev.filter(user => user.id !== userId));
        
        // Add back to contacts if it was a contact
        const wasContact = existingMembers.some(member => member.addedUser.id === userId);
        if (wasContact) {
            fetchContacts(); // Refresh contacts list
        }
    };

    const handleAddMembers = async () => {
        if (selectedUsers.length === 0) return;

        try {
            setAdding(true);
            const memberIds = selectedUsers.map(user => user.userId);
            await apiService.post(`${GROUP_PATHS.ADD_MEMBERS}`, {groupId:groupId,friendIds: memberIds });
            toast.success('Members added successfully');
            onClose(true);
        } catch (error) {
            console.error('Error adding members:', error);
            toast.error('Failed to add members');
        } finally {
            setAdding(false);
        }
    };

    const handleClose = () => {
        setSearchTerm('');
        setSearchResults([]);
        setSelectedUsers([]);
        setActiveTab(0);
        onClose();
    };

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            fullScreen={fullScreen}
            maxWidth="sm"
            fullWidth
        >
            <DialogTitle>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="h6">Add Members</Typography>
                    <IconButton size="small" onClick={handleClose}>
                        <CloseIcon />
                    </IconButton>
                </Box>
            </DialogTitle>

            <Divider />

            <DialogContent>
                <Tabs
                    value={activeTab}
                    onChange={(e, newValue) => setActiveTab(newValue)}
                    sx={{ mb: 2 }}
                >
                    <Tab 
                        icon={<ContactIcon />} 
                        label="Contacts" 
                        iconPosition="start"
                    />
                    <Tab 
                        icon={<SearchIcon />} 
                        label="Search" 
                        iconPosition="start"
                    />
                </Tabs>

                {/* Selected Users */}
                {selectedUsers.length > 0 && (
                    <Box sx={{ mb: 2 }}>
                        <Typography variant="subtitle2" gutterBottom>
                            Selected Users ({selectedUsers.length})
                        </Typography>
                        <List dense>
                            {selectedUsers.map(user => (
                                <ListItem
                                    key={user.id}
                                    secondaryAction={
                                        <IconButton 
                                            edge="end" 
                                            onClick={() => handleRemoveUser(user.id)}
                                            size="small"
                                        >
                                            <CloseIcon />
                                        </IconButton>
                                    }
                                >
                                    <ListItemAvatar>
                                        <Avatar
                                            src={user.profilePicture ? getImageUrl(user.profilePicture) : undefined}
                                        >
                                            {user.firstName?.[0]}
                                        </Avatar>
                                    </ListItemAvatar>
                                    <ListItemText
                                        primary={`${user.firstName} ${user.lastName || ''}`}
                                        secondary={user.email}
                                    />
                                </ListItem>
                            ))}
                        </List>
                    </Box>
                )}

                <Divider sx={{ my: 2 }} />

                {activeTab === 0 ? (
                    // Contacts Tab
                    <Box>
                        {loadingContacts ? (
                            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
                                <CircularProgress size={24} />
                            </Box>
                        ) : contacts.length > 0 ? (
                            <List dense>
                                {contacts.map(contact => (
                                    <ListItem
                                        key={contact.id}
                                        secondaryAction={
                                            <IconButton 
                                                edge="end" 
                                                onClick={() => handleSelectContact(contact)}
                                                color="primary"
                                                size="small"
                                            >
                                                <AddIcon />
                                            </IconButton>
                                        }
                                    >
                                        <ListItemAvatar>
                                            <Avatar
                                                src={contact.contactUser.profilePicture ? getImageUrl(contact.contactUser.profilePicture) : undefined}
                                            >
                                                {contact.contactUser.firstName?.[0]}
                                            </Avatar>
                                        </ListItemAvatar>
                                        <ListItemText
                                            primary={`${contact.contactUser.firstName} ${contact.contactUser.lastName || ''}`}
                                            secondary={contact.contactUser.email}
                                        />
                                    </ListItem>
                                ))}
                            </List>
                        ) : (
                            <Box sx={{ textAlign: 'center', p: 2 }}>
                                <Typography color="textSecondary">
                                    No contacts available
                                </Typography>
                            </Box>
                        )}
                    </Box>
                ) : (
                    // Search Tab
                    <Box>
                        <TextField
                            fullWidth
                            placeholder="Search by name or email"
                            value={searchTerm}
                            onChange={handleSearchChange}
                            InputProps={{
                                startAdornment: (
                                    <InputAdornment position="start">
                                        <SearchIcon />
                                    </InputAdornment>
                                ),
                                endAdornment: searching && (
                                    <InputAdornment position="end">
                                        <CircularProgress size={20} />
                                    </InputAdornment>
                                )
                            }}
                        />

                        {searchResults.length > 0 && (
                            <List dense sx={{ mt: 2 }}>
                                {searchResults.map(user => (
                                    <ListItem
                                        key={user.id}
                                        secondaryAction={
                                            <IconButton 
                                                edge="end" 
                                                onClick={() => handleSelectUser(user)}
                                                color="primary"
                                                size="small"
                                            >
                                                <AddIcon />
                                            </IconButton>
                                        }
                                    >
                                        <ListItemAvatar>
                                            <Avatar
                                                src={user.profilePicture ? getImageUrl(user.profilePicture) : undefined}
                                            >
                                                {user.firstName?.[0]}
                                            </Avatar>
                                        </ListItemAvatar>
                                        <ListItemText
                                            primary={`${user.firstName} ${user.lastName || ''}`}
                                            secondary={user.email}
                                        />
                                    </ListItem>
                                ))}
                            </List>
                        )}

                        {searchTerm && !searching && searchResults.length === 0 && (
                            <Box sx={{ textAlign: 'center', mt: 2 }}>
                                <Typography color="textSecondary">
                                    No users found
                                </Typography>
                            </Box>
                        )}
                    </Box>
                )}
            </DialogContent>

            <DialogActions sx={{ p: 2, pt: 0 }}>
                <Button onClick={handleClose}>
                    Cancel
                </Button>
                <Button
                    variant="contained"
                    onClick={handleAddMembers}
                    disabled={selectedUsers.length === 0 || adding}
                >
                    {adding ? 'Adding...' : 'Add Members'}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default AddGroupMemberDialog; 