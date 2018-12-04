import queryString from 'query-string';
import _ from 'lodash';
import {API} from '../utils';
import {apiRequest as request} from '../services';

const INITIAL_STATE = {
  displayList: '',
  pageSize: 5,
  currentPage: 1,
};

export default {
  namespace: 'userModel',
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
    *getUserList(payloadObj, {put, call, select}) {
      yield put({
        type: 'updateState',
        payload: {
          awaitingResponse: true,
        },
      });
      const {userModel} = yield select(state => state);
      const params = {
        currentPage: userModel.currentPage,
        pageSize: userModel.pageSize,
      };
      const response = yield call(request.to, {
        url: API.processGetUserList + '?' + queryString.stringify(params),
        method: 'get',
      });

      if (response) {
        if (response.data) {
          const statusOK = typeof response.data === 'object';
          if (statusOK) {
            yield put({
              type: 'updateState',
              payload: {
                displayList: _.cloneDeep(response.data),
              },
            });
          }
        } else if (response.err) {
          // throw new Error(`API Failed，${response.err.message}`);
        }
        yield put({
          type: 'updateState',
          payload: {
            awaitingResponse: false,
          },
        });
      }
    },
  },
};
