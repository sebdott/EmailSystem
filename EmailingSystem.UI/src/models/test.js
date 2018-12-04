import queryString from 'query-string';
import _ from 'lodash';
import {notification} from 'antd';
import {API} from '../utils';
import {apiRequest as request} from '../services';

const INITIAL_STATE = {
  displayList: '',
  records: 50,
};

export default {
  namespace: 'testModel',
  state: INITIAL_STATE,
  reducers: {
    updateState(state, {payload}) {
      return {
        ...state,
        ...payload,
      };
    },
    removeState(state, {payload}) {
      const newState = _.omit(state, payload);
      return {
        ...newState,
      };
    },
    initializeState(state, {payload}) {
      const initialStates = _.pick(INITIAL_STATE, payload);
      return {
        ...state,
        ...initialStates,
      };
    },
    initializeAll(state, {payload}) {
      let newState = {};
      if (payload) {
        newState = _.omit(INITIAL_STATE, payload);
      } else {
        newState = INITIAL_STATE;
      }

      return {
        ...state,
        ...newState,
      };
    },
  },
  effects: {
    *ResetTestData(payloadObj, {put, call, select}) {
      yield put({
        type: 'updateState',
        payload: {
          awaitingResponse: true,
        },
      });
      const {testModel} = yield select(state => state);
      const params = {
        records: testModel.records,
      };
      const response = yield call(request.to, {
        url: API.processResetTestData + '?' + queryString.stringify(params),
        method: 'post',
      });

      if (response) {
        const {isSuccess} = response.data;
        if (isSuccess) {
          notification['success']({
            message: 'Initiate Set Status Update Process Success',
          });
        } else {
          notification['error']({
            message: 'Initiate Set Status Update Process Error',
          });
        }
      }

      yield put({
        type: 'updateState',
        payload: {
          awaitingResponse: false,
        },
      });
    },
    *InitiateSendEmailProcess(payloadObj, {put, call, select}) {
      yield put({
        type: 'updateState',
        payload: {
          awaitingResponse: true,
        },
      });
      const response = yield call(request.to, {
        url: API.InitiateSendEmailProcess,
        method: 'post',
      });

      if (response) {
        const {isSuccess} = response.data;
        if (isSuccess) {
          notification['success']({
            message: 'Initiate Set Status Update Process Success',
          });
        } else {
          notification['error']({
            message: 'Initiate Set Status Update Process Error',
          });
        }
      }

      yield put({
        type: 'updateState',
        payload: {
          awaitingResponse: false,
        },
      });
    },
    *InitiateSetStatusUpdateProcess(payloadObj, {put, call, select}) {
      yield put({
        type: 'updateState',
        payload: {
          awaitingResponse: true,
        },
      });
      const response = yield call(request.to, {
        url: API.InitiateSetStatusUpdateProcess,
        method: 'post',
      });

      if (response) {
        const {isSuccess} = response.data;
        if (isSuccess) {
          notification['success']({
            message: 'Initiate Set Status Update Process Success',
          });
        } else {
          notification['error']({
            message: 'Initiate Set Status Update Process Error',
          });
        }
      }

      yield put({
        type: 'updateState',
        payload: {
          awaitingResponse: false,
        },
      });
    },
  },
};
