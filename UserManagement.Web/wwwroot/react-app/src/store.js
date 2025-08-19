import { configureStore, createSlice } from '@reduxjs/toolkit';

const userSlice = createSlice({
    name: 'user',
    initialState: { selectedUser: null },
    reducers: {
    setSelectedUser: (state, action) => {
        state.selectedUser = action.payload;
        },
    },
});

export const { setSelectedUser } = userSlice.actions;

export const store = configureStore({
    reducer: {
        user: userSlice.reducer,
    },
});