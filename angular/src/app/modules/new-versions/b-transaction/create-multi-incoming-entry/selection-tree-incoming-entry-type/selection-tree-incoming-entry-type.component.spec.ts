import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectionTreeIncomingEntryTypeComponent } from './selection-tree-incoming-entry-type.component';

describe('SelectionTreeIncomingEntryTypeComponent', () => {
  let component: SelectionTreeIncomingEntryTypeComponent;
  let fixture: ComponentFixture<SelectionTreeIncomingEntryTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SelectionTreeIncomingEntryTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectionTreeIncomingEntryTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
